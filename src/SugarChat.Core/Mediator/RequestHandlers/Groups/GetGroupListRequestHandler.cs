using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Groups;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Groups
{
    public class GetGroupListRequestHandler : IRequestHandler<GetGroupsOfUserRequest, GetGroupsOfUserResponse>
    {
        private readonly IGroupService _groupService;

        public GetGroupListRequestHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }
        public async Task<GetGroupsOfUserResponse> Handle(IReceiveContext<GetGroupsOfUserRequest> context, CancellationToken cancellationToken)
        {
            return await _groupService.GetGroupsOfUserAsync(context.Message, cancellationToken).ConfigureAwait(false);
        }
    }
}
