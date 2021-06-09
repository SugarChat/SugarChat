using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Message.Commands.Friends;
using SugarChat.Message.Events.Users;

namespace SugarChat.Core.Mappings
{
    public class FriendMapping: Profile
    {
        public FriendMapping()
        {
            CreateMap<AddFriendCommand, Friend>();
            CreateMap<AddFriendCommand, FriendAddedEvent>();
            CreateMap<RemoveFriendCommand, FriendRemovedEvent>();
        }
    }
}