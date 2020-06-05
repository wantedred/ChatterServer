using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WebChatterServer.Hubs
{
    public class MessageHub : Hub
    {
        readonly Dictionary<string, string> UserList = new Dictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, "Public");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine(Context.ConnectionId);
            UserList.Remove(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Public");
            await base.OnDisconnectedAsync(exception);
        }

        public Task RegisterUsername(string username)
        {
            string userTaken = UserList[Context.ConnectionId];
            if (String.IsNullOrEmpty(userTaken))
            {
                UserList.Add(Context.ConnectionId, username);
            }
            return Clients.Caller.SendAsync("UsernameRecieved", String.IsNullOrEmpty(userTaken) ? true : false);
        }

        public async Task NewMessage(Message msg)
        {
            Console.WriteLine(msg.Text);
            await Clients.All.SendAsync("MessageReceived", msg);
        }
    }
}
