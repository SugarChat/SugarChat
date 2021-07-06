using Mediator.Net.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Requests.Groups
{
    public class GetGroupByCustomPropertiesRequest : IRequest
    {
        public string UserId { get; set; }
        public Dictionary<string, string> CustomPropertys { get; set; }
    }
}
