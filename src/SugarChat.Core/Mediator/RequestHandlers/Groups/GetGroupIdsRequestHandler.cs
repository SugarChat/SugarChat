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
    public class GetGroupIdsRequestHandler : IRequestHandler<GetGroupIdsOfUserRequest, SugarChatResponse<IEnumerable<string>>>
    {
        private readonly IGroupService _groupService;

        public GetGroupIdsRequestHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }
        public async Task<SugarChatResponse<IEnumerable<string>>> Handle(IReceiveContext<GetGroupIdsOfUserRequest> context, CancellationToken cancellationToken)
        {
            var response = await _groupService.GetGroupIdsOfUserAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<IEnumerable<string>>() { Data = response.GroupIds };
        }
    }
}
