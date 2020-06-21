using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebChatterServer.Models;
using WebChatterServer.Models.Database.BindingModels;

namespace WebChatterServer.Hubs
{
    public partial class MainHub
    {

        public async Task<bool> RegisterUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }

            UserStatus userStatus =
                await _mainContext.UserStatuses.FirstOrDefaultAsync(user => user.Username== username);
            
            if (userStatus != null)
            {
                Console.WriteLine("Found user with name already returning");
                return false;
            }

            Console.WriteLine("Didn't find a user with the name already");
            await _mainContext.UserStatuses.AddAsync(new UserStatus
            {
                Username = username,
                ConnectionId = Context.ConnectionId,
                Status = true
            });
            await _mainContext.SaveChangesAsync();
            
            await Clients.Group("Public").SendAsync("NewMessage", new Message { Username = "System", Text = "Please welcome " + username + " to the chat!", Type = "received", Date = DateTime.Now });
            await Clients.All.SendAsync("ReceiveUsername", new ReceiveUsername { NewUsername = username });
            return true;
        }

        public async Task<bool> RequestAllUsers()
        {
            if (UserList.Count <= 0)
            {
                return false;
            }

            IQueryable<UserStatus> userStatus = _mainContext.UserStatuses.Where(user => user.Status == true).AsNoTracking();
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
            return true;
        }

        public async Task<bool> NewMessage(Message message)
        {
            if (message.Text.Length < 4)
            {
                return false;
            }

            if (message.Username.Length < 4)
            {
                return false;
            }
            
            await Clients.All.SendAsync("NewMessage", message);
            return true;
        }
    }
}
