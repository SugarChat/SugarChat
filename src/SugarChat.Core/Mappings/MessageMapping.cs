using AutoMapper;
using SugarChat.Shared.Dtos;

namespace SugarChat.Core.Mappings
{
    public class MessageMapping: Profile
    {
        public MessageMapping()
        {
            CreateMap<Domain.Message, MessageDto>();           
        }
        
    }
}
