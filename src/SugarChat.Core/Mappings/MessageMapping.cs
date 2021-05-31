using AutoMapper;
using SugarChat.Message.Command;
using SugarChat.Message.Messages.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Mappings
{
    public class MessageMapping : Profile
    {
        public MessageMapping()
        {
            CreateMap<SendMessageCommand, MessageSentEvent>();
        }
    }
}
