using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Dtos;

namespace SugarChat.Core.Mappings
{
    public class Group2Mapping : Profile
    {
        public Group2Mapping()
        {
            CreateMap<AddGroupCommand, Group2>()
                .ForMember(dest => dest.GroupUsers, opt => opt.Ignore())
                .ForMember(dest => dest.Messages, opt => opt.Ignore());
            CreateMap<Group, Group2>()
                .ForMember(dest => dest.GroupUsers, opt => opt.Ignore())
                .ForMember(dest => dest.Messages, opt => opt.Ignore());
            CreateMap<GroupUser, GroupUser2>();
            CreateMap<SendMessageCommand, Message2>();
            CreateMap<Domain.Message, Message2>();
            CreateMap<Group2, GroupDto>();
        }
    }
}
