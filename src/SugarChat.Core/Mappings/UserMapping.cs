using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Dtos;
using SugarChat.Message.Events.Users;

namespace SugarChat.Core.Mappings
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<AddUserCommand, User>();
            CreateMap<AddUserCommand, UserAddedEvent>();
            CreateMap<UpdateUserCommand, User>();
            CreateMap<UpdateUserCommand, UserUpdatedEvent>();
            CreateMap<RemoveUserCommand, UserRemovedEvent>();
            CreateMap<BatchAddUsersCommand, UsersBatchAddedEvent>();
        }
    }
}
