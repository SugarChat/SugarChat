using AutoMapper;
using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Events.GroupUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Mappings
{
    public class GroupUserMapping : Profile
    {
        public GroupUserMapping()
        {
            CreateMap<JoinGroupCommand, GroupJoinedEvent>();
            CreateMap<QuitGroupCommand, GroupQuittedEvent>();
            CreateMap<ChangeGroupOwnerCommand, GroupOwnerChangedEvent>();
            CreateMap<AddGroupMemberCommand, GroupMemberAddedEvent>();
            CreateMap<DeleteGroupMemberCommand, GroupMemberDeletedEvent>();
            CreateMap<SetMessageRemindTypeCommand, MessageRemindTypeSetEvent>();
        }
    }
}
