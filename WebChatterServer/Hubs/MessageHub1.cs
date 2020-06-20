using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebChatterServer.Hubs
{
    public partial class MessageHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("New Connection: " + Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, "Public");
            await base.OnConnectedAsync();
        }
    }
}
