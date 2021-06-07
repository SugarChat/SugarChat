using AutoMapper;
using SugarChat.Message.Commands.Conversations;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Events.Conversations;
using SugarChat.Message.Events.Messages;

namespace SugarChat.Core.Mappings
{
    public class ConversationMapping : Profile
    {
        public ConversationMapping()
        {
            CreateMap<RemoveConversationCommand, ConversationRemovedEvent>();
            CreateMap<SetMessageReadByUserBasedOnMessageIdCommand, MessageReadSetByUserBasedOnMessageIdEvent>();
        }
    }
}
