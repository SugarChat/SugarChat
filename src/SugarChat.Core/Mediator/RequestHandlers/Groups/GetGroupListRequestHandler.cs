using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Groups;
using SugarChat.Message.Requests;
using SugarChat.Shared.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Shared.Paging;

namespace SugarChat.Core.Mediator.RequestHandlers.Groups
{
    public class GetGroupListRequestHandler : IRequestHandler<GetPagedGroupsOfUserRequest, SugarChatResponse<PagedResult<GroupDto>>>
    {
        private readonly IGroupService _groupService;

        public GetGroupListRequestHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }
        public async Task<SugarChatResponse<PagedResult<GroupDto>>> Handle(IReceiveContext<GetPagedGroupsOfUserRequest> context, CancellationToken cancellationToken)
        {
            var response = await _groupService.GetPagedGroupsOfUserAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<PagedResult<GroupDto>>() { Data = response.Groups };
        }
    }
}
