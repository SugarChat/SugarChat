using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Commands.Users;
using SugarChat.Shared.Dtos;
using SugarChat.Shared.Paging;

namespace SugarChat.Core.Mappings
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<AddGroupCommand, Group>();
            CreateMap<AddUserCommand, User>();
            CreateMap<UpdateUserCommand, User>();
            CreateMap<User, UserDto>();
            CreateMap<Group, GroupDto>();
        }
    }
}