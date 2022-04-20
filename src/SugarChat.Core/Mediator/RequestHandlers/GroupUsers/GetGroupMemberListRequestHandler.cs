using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Requests;
using SugarChat.Message.Dtos.GroupUsers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Basic;

namespace SugarChat.Core.Mediator.RequestHandlers.GroupUsers
{
    public class GetGroupMemberListRequestHandler : IRequestHandler<GetMembersOfGroupRequest, SugarChatResponse<IEnumerable<GroupUserDto>>>
    {
        private readonly IGroupUserService _groupUserService;
        public GetGroupMemberListRequestHandler(IGroupUserService groupUserService)
        {
            _groupUserService = groupUserService;
        }

        public async Task<SugarChatResponse<IEnumerable<GroupUserDto>>> Handle(IReceiveContext<GetMembersOfGroupRequest> context, CancellationToken cancellationToken)
        {
            var response = await _groupUserService.GetGroupMembersByIdAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<IEnumerable<GroupUserDto>>() { Data = response.Members };
        }
    }
}
