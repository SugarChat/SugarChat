using AutoMapper;
using SugarChat.Message.Commands;
using SugarChat.Message.Commands.Conversations;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Dtos;
using SugarChat.Message.Events.Conversations;
using SugarChat.Message.Events.Messages;

namespace SugarChat.Core.Mappings
{
    public class MessageMapping : Profile
    {
        public MessageMapping()
        {
            CreateMap<Domain.Message, MessageDto>();
            CreateMap<SendMessageCommand, Domain.Message>();
            CreateMap<SendMessageCommand, MessageSavedEvent>();
            CreateMap<RevokeMessageCommand, MessageRevokedEvent>();
            CreateMap<SetMessageReadByUserBasedOnGroupIdCommand, MessageReadSetByUserBasedOnGroupIdEvent>();
            CreateMap<SetMessageReadByUserIdsBasedOnGroupIdCommand, MessageReadSetByUserIdsBasedOnGroupIdEvent>();
        }
    }
}