using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Events.Users;
using SugarChat.Shared.Dtos;

namespace SugarChat.Core.Mappings
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<User, UserDto>();
            CreateMap<UpdateUserCommand, User>();
            CreateMap<UpdateUserCommand, UserUpdatedEvent>();
        }
    }
}
