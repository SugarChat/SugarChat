using Mediator.Net.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Commands.Messages
{
    public class TranslateMessageCommand : IdRequiredCommand
    {
        public string MessageId { get; set; }

        public string LanguageCode { get; set; }

        public string CreatedBy { get; set; }
    }
}
