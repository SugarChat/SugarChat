using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Models
{
    public class GetConnectionUrlModel
    {
        public string Identifier { get; set; }
        public Dictionary<string, string> Claims { get; set; }
    }
}
