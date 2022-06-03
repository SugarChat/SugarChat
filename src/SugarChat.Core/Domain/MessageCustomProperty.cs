using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Domain
{
    public class MessageCustomProperty : Entity
    {
        public MessageCustomProperty()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string MessageId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
