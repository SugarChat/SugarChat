using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Message.Commands.Groups;

namespace SugarChat.Core.Mappings
{
    public class GroupMapping : Profile
    {
        public GroupMapping()
        {
            CreateMap<AddGroupCommand, Group>();
        }
    }
}