using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebChatterServer.Hubs
{
    public class Message
    {
        public string ClientId { get; set; }

        public string Type { get; set; }

        public string Text { get; set; }

        public DateTime Date { get; set; }
    }
}
