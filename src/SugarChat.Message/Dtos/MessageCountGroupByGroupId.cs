using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Dtos
{
    public class MessageCountGroupByGroupId
    {
        public string GroupId { get; set; }

        public int Count { get; set; }
    }
}
