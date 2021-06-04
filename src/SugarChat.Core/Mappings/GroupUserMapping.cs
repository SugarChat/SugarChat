using AutoMapper;
using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Events.GroupUsers;

namespace SugarChat.Core.Mappings
{
    public class GroupUserMapping : Profile
    {
        public GroupUserMapping()
        {
            CreateMap<SetGroupMemberCustomFieldCommand, GroupMemberCustomFieldBeSetEvent>();           
        }
    }
}
