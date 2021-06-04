using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Message.Commands.Friends;

namespace SugarChat.Core.Mappings
{
    public class FriendMapping: Profile
    {
        public FriendMapping()
        {
            CreateMap<AddFriendCommand, Friend>();
        }
    }
}