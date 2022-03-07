using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Groups;
using SugarChat.Message.Requests;
using SugarChat.Message.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Paging;
using SugarChat.Message.Basic;

namespace SugarChat.Core.Mediator.RequestHandlers.Groups
{
    public class GetGroupListRequestHandler : IRequestHandler<GetGroupsOfUserRequest, SugarChatResponse<PagedResult<GroupDto>>>
    {
        private readonly IGroupService _groupService;

        public GetGroupListRequestHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }
        public async Task<SugarChatResponse<PagedResult<GroupDto>>> Handle(IReceiveContext<GetGroupsOfUserRequest> context, CancellationToken cancellationToken)
        {
            var response = await _groupService.GetGroupsOfUserAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<PagedResult<GroupDto>>() { Data = response.Groups };
        }
    }
}
