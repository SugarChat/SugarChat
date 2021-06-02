using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Commands.Users;

namespace SugarChat.Core.Mappings
{
    public class GroupMapping : Profile
    {
        public GroupMapping()
        {
            CreateMap<AddGroupCommand, Group>();
            CreateMap<AddUserCommand, User>();
            CreateMap<UpdateUserCommand, User>();
        }
    }
}