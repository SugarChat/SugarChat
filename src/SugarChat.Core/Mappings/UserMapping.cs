using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Shared.Dtos;

namespace SugarChat.Core.Mappings
{
    public class UserMapping: Profile
    {
        public UserMapping()
        {
            CreateMap<UserDto, User>();
            CreateMap<User, UserDto>();
        }
    }
}
