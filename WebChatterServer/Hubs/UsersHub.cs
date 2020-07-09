using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebChatterServer.Models;
using WebChatterServer.Models.Database.BindingModels;
using WebChatterServer.Models.ResourceModels;

namespace WebChatterServer.Hubs
{
    public partial class MainHub
    {
        public async Task<Response> RegisterUsername(User user)
        {
            if (string.IsNullOrEmpty(user.Username))
            {
                return new Response { Success = false, ErrorMessage = "Please give an username" };
            }

            UserStatus userStatus = string.IsNullOrEmpty(user.UserId)
                ? await _mainContext.UserStatuses.FirstOrDefaultAsync(u => u.Username == user.Username)
                : await _mainContext.UserStatuses.FirstOrDefaultAsync(id => id.ConnectionId == user.UserId);

            if (userStatus != null)
            {
                return new Response { Success = false, ErrorMessage = "Username is already in use" };
            }

            if (string.IsNullOrEmpty(user.UserId))
            {
                await _mainContext.UserStatuses.AddAsync(new UserStatus
                {
                    Username = user.Username,
                    ConnectionId = Context.ConnectionId,
                    Status = true
                });
            }
            else
            {
                userStatus.ConnectionId = Context.ConnectionId;
                userStatus.Status = true;
            }
            await _mainContext.SaveChangesAsync();

            await Clients.Group("Public").SendAsync("NewMessage", new Message { Username = "System", Text = "Please welcome " + user.Username + " to the chat!", Type = "received", Date = DateTime.Now });

            await Clients.All.SendAsync("ReceiveUsername", new ReceiveUsername { NewUsername = user.Username });
            return new Response { Success = true };
        }

        public async Task<Response> RequestAllUsers()
        {
            IQueryable<UserStatus> userStatus = _mainContext.UserStatuses.Where(user => user.Status == true).AsNoTracking();

            if (userStatus == null)
            {
                return new Response { Success = false };
            }

            List<User> users = new List<User>();
            foreach (var data in userStatus)
            {
                users.Add(new User
                {
                    UserId = data.ConnectionId,
                    Username = data.Username
                });
            }

            await Clients.Caller.SendAsync("ReceiveAllUsers", new { users });
            return new Response { Success = true };
        }
    }
}
