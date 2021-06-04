using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Events.Groups;
using SugarChat.Shared.Dtos;

namespace SugarChat.Core.Mappings
{
    public class GroupMapping : Profile
    {
        public GroupMapping()
        {
            CreateMap<AddGroupCommand, Group>();
            CreateMap<UpdateGroupProfileCommand, Group>();
            CreateMap<Group, GroupDto>();
            CreateMap<UpdateGroupProfileCommand, GroupAddedEvent>();
            CreateMap<UpdateGroupProfileCommand, GroupProfileUpdatedEvent>();
        }
    }
}