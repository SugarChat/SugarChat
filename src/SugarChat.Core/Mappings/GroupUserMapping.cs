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
            CreateMap<JoinGroupCommand, GroupJoinedEvent>();
            CreateMap<QuitGroupCommand, GroupQuittedEvent>();
            CreateMap<ChangeGroupOwnerCommand, GroupOwnerChangedEvent>();
            CreateMap<AddGroupMemberCommand, GroupMemberAddedEvent>();
            CreateMap<DeleteGroupMemberCommand, GroupMemberDeletedEvent>();
            CreateMap<SetMessageRemindTypeCommand, MessageRemindTypeSetEvent>();
            CreateMap<SetGroupMemberRoleCommand, GroupMemberRoleSetEvent>();
        }
    }
}