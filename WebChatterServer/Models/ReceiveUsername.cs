using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebChatterServer.Models
{
    public class ReceiveUsername
    {
        
        public string NewUsername { get; set; }

        public bool RemoveUser { get; set; } = false;

    }
}
