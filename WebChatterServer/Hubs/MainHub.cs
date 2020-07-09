using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebChatterServer.Models;
using WebChatterServer.Models.Database.BindingModels;
using WebChatterServer.Models.Database.Contexts;

namespace WebChatterServer.Hubs
{
    public partial class MainHub : Hub
    {

        private readonly MainContext _mainContext;
        
        public MainHub(MainContext mainContext)
        {
            _mainContext = mainContext;
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Public");
            await base.OnConnectedAsync();
        }
        
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("Connection disconnecting: " + Context.ConnectionId);
            UserStatus userStatus =
                await _mainContext.UserStatuses.FirstOrDefaultAsync(user => user.ConnectionId == Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Public");
            if (userStatus == null)
            {
                Console.WriteLine("User connection is null");
                await base.OnDisconnectedAsync(exception);
                return;
            }

            userStatus.Status = false;
            await _mainContext.SaveChangesAsync();
            await Clients.All.SendAsync("ReceiveUsername", new ReceiveUsername { NewUsername = userStatus.Username, RemoveUser = true });
            await base.OnDisconnectedAsync(exception);
        }
    }
}