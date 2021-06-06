using AutoMapper;
using SugarChat.Message.Commands;
using SugarChat.Message.Commands.Conversations;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Events.Conversations;
using SugarChat.Message.Events.Messages;
using SugarChat.Message.Messages.Events;
using SugarChat.Shared.Dtos;

namespace SugarChat.Core.Mappings
{
    public class MessageMapping: Profile
    {
        public MessageMapping()
        {
            CreateMap<Domain.Message, MessageDto>();         
            CreateMap<SendMessageCommand, MessageSentEvent>();
        }
    }
}