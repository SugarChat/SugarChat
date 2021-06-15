using AutoMapper;
using SugarChat.Message.Commands.Conversations;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Commands.SignalR;
using SugarChat.Message.Events.Conversations;
using SugarChat.Message.Events.Messages;
using SugarChat.Message.Events.SignalR;

namespace SugarChat.Core.Mappings
{
    public class ConversationMapping : Profile
    {
        public ConversationMapping()
        {
            CreateMap<RemoveConversationCommand, ConversationRemovedEvent>();
            CreateMap<AddToConversationsCommand, AddedToConversationsEvent>();
            CreateMap<SetMessageReadByUserBasedOnMessageIdCommand, MessageReadSetByUserBasedOnMessageIdEvent>();
        }
    }
}
