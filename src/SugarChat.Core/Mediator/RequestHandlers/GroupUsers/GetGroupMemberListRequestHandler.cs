using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.GroupUsers
{
    public class GetGroupMemberListRequestHandler : IRequestHandler<GetMembersOfGroupRequest, GetMembersOfGroupResponse>
    {
        private readonly IGroupUserService _groupUserService;
        public GetGroupMemberListRequestHandler(IGroupUserService groupUserService)
        {
            _groupUserService = groupUserService;
        }

        public async Task<GetMembersOfGroupResponse> Handle(IReceiveContext<GetMembersOfGroupRequest> context, CancellationToken cancellationToken)
        {
            return await _groupUserService.GetGroupMembersByIdAsync(context.Message, cancellationToken).ConfigureAwait(false);
        }
    }
}
