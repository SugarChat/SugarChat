using AutoMapper;
using SugarChat.Message.Commands.Conversations;
using SugarChat.Message.Events.Conversations;
using SugarChat.Shared.Dtos;

namespace SugarChat.Core.Mappings
{
    public class MessageMapping: Profile
    {
        public MessageMapping()
        {
            CreateMap<Domain.Message, MessageDto>();   
            CreateMap<SetMessageAsReadCommand, MessageReadedEvent>();
        }        
    }
}
