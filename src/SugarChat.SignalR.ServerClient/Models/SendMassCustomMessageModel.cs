using SugarChat.SignalR.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SugarChat.SignalR.Server.Models
{
    public class SendMassCustomMessageModel
    {
        public SendWay SendWay { get; set; }

        public string Method { get; set; }

        public object[] Messages { get; set; }

        public IReadOnlyList<string> SendTos { get; set; }

    }
}
