using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WebChatterServer.Models;

namespace WebChatterServer.Hubs
{
    public partial class MessageHub : Hub
    {
        public static readonly Dictionary<string, string> UserList = new Dictionary<string, string>();

        public MessageHub()
        {
            
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine(Context.ConnectionId);
            if (UserList.ContainsKey(Context.ConnectionId))
            {
                string username = UserList[Context.ConnectionId];
                UserList.Remove(Context.ConnectionId);
                await Clients.All.SendAsync("ReceiveUsername", new ReceiveUsername { NewUsername = username, RemoveUser = true });
            }
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Public");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task<bool> RegisterUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }

            bool usernameFree = !UserList.ContainsValue(username);
            if (usernameFree)
            {
                UserList.Add(Context.ConnectionId, username);
                await Clients.Group("Public").SendAsync("NewMessage", new Message { Username = "System", Text = "Please welcome " + username + " to the chat!", Type = "received", Date = DateTime.Now });
                await Clients.All.SendAsync("ReceiveUsername", new ReceiveUsername { NewUsername = username });
            }
            return usernameFree;
        }

        public async Task<bool> RequestAllUsers()
        {
            Console.WriteLine("Trying to get all users");
            if (UserList.Count <= 0)
            {
                Console.WriteLine("Count less than zero");
                return false;
            }
            List<User> users = new List<User>();
            foreach(KeyValuePair<string, string> data in UserList)
            {
                users.Add(new User
                {
                    UserId = data.Key,
                    Username = data.Value
                });
            }
            await Clients.Caller.SendAsync("ReceiveAllUsers", new { users });
            return true;
        }

        public async Task<bool> NewMessage(Message msg)
        {
            Console.WriteLine(msg.Text);
            await Clients.All.SendAsync("NewMessage", msg);
            return true;
        }
    }
}
