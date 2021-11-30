using Mediator.Net.Contracts;
using SugarChat.Message.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Requests.Groups
{
    public class GetGroupByCustomPropertiesRequest : IRequest, INeedUserExist
    {
        public string UserId { get; set; }
        public Dictionary<string, string> CustomProperties { get; set; }
        public bool SearchAllGroup { get; set; }
    }
}
