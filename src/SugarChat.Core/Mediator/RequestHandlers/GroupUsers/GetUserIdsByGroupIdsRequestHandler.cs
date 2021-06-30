using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Requests.GroupUsers;
using SugarChat.Shared.Dtos.GroupUsers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.GroupUsers
{
    public class GetUserIdsByGroupIdsRequestHandler : IRequestHandler<GetUserIdsByGroupIdsRequest, SugarChatResponse<IEnumerable<string>>>
    {
        private readonly IGroupUserService _groupUserService;

        public GetUserIdsByGroupIdsRequestHandler(IGroupUserService groupUserService)
        {
            _groupUserService = groupUserService;
        }

        public async Task<SugarChatResponse<IEnumerable<string>>> Handle(IReceiveContext<GetUserIdsByGroupIdsRequest> context, CancellationToken cancellationToken)
        {
            var response = await _groupUserService.GetUsersByGroupIdsAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<IEnumerable<string>>() { Data = response.UserIds };
        }
    }
}
