using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Requests.GroupUsers;
using SugarChat.Message.Dtos.GroupUsers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Basic;
using SugarChat.Core.Services;

namespace SugarChat.Core.Mediator.RequestHandlers.GroupUsers
{
    public class GetUserIdsByGroupIdsRequestHandler : IRequestHandler<GetUserIdsByGroupIdsRequest, SugarChatResponse<IEnumerable<string>>>
    {
        private readonly IGroupUserService _groupUserService;
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public GetUserIdsByGroupIdsRequestHandler(IGroupUserService groupUserService, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _groupUserService = groupUserService;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }

        public async Task<SugarChatResponse<IEnumerable<string>>> Handle(IReceiveContext<GetUserIdsByGroupIdsRequest> context, CancellationToken cancellationToken)
        {
            _backgroundJobClientProvider.Enqueue(() => _groupUserService.GetUsersByGroupIdsAsync2(context.Message, cancellationToken));
            var response = await _groupUserService.GetUsersByGroupIdsAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<IEnumerable<string>>() { Data = response.UserIds };
        }
    }
}
