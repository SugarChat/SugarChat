using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Events.GroupUsers;
using SugarChat.Shared.Dtos;

namespace SugarChat.Core.Mappings
{
    public class GroupUserMapping : Profile
    {
        public GroupUserMapping()
        {
            CreateMap<AddUserToGroupCommand, GroupUser>();
            CreateMap<AddGroupUserDto, GroupUser>();
            CreateMap<JoinGroupCommand, GroupUser>();

            CreateMap<SetGroupMemberCustomFieldCommand, GroupMemberCustomFieldSetEvent>();
            CreateMap<JoinGroupCommand, GroupJoinedEvent>();
            CreateMap<QuitGroupCommand, GroupQuittedEvent>();
            CreateMap<ChangeGroupOwnerCommand, GroupOwnerChangedEvent>();
            CreateMap<AddGroupMemberCommand, GroupMemberAddedEvent>();
            CreateMap<RemoveGroupMemberCommand, GroupMemberRemovedEvent>();
            CreateMap<SetMessageRemindTypeCommand, MessageRemindTypeSetEvent>();
            CreateMap<SetGroupMemberRoleCommand, GroupMemberRoleSetEvent>();
        }
    }
}