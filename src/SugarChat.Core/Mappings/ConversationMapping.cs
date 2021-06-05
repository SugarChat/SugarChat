using AutoMapper;
using SugarChat.Message.Commands.Conversations;
using SugarChat.Message.Events.Conversations;

namespace SugarChat.Core.Mappings
{
    public class ConversationMapping : Profile
    {
        public ConversationMapping()
        {
            CreateMap<DeleteConversationCommand, ConversationDeletedEvent>();           
        }
    }
}
