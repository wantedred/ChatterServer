using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebChatterServer.Models;
using WebChatterServer.Models.Database.BindingModels;
using WebChatterServer.Models.ResourceModels;

namespace WebChatterServer.Hubs
{
    public partial class MainHub
    {

        public async Task<Response> NewMessage(Message message)
        {
           
            if (message.Text.Trim().Length <= 0)
            {
                return new Response{ Success = false, ErrorMessage = "Message has can't be empty"};
            }

            if (String.IsNullOrEmpty(message.Username))
            {
                return new Response{ Success = false, ErrorMessage = "You must supply a display name before sending messages"};
            }
            
            await Clients.All.SendAsync("NewMessage", message);
            return new Response { Success = true };
        }
    }
}
