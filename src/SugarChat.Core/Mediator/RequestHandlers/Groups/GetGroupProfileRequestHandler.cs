using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Groups;
using SugarChat.Message.Requests.Groups;
using SugarChat.Message.Dtos;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Basic;
using SugarChat.Core.Services;

namespace SugarChat.Core.Mediator.RequestHandlers.Groups
{
    public class GetGroupProfileRequestHandler : IRequestHandler<GetGroupProfileRequest, SugarChatResponse<GroupDto>>
    {
        private readonly IGroupService _groupService;
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public GetGroupProfileRequestHandler(IGroupService groupService, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _groupService = groupService;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }
        public async Task<SugarChatResponse<GroupDto>> Handle(IReceiveContext<GetGroupProfileRequest> context, CancellationToken cancellationToken)
        {
            _backgroundJobClientProvider.Enqueue(() => _groupService.GetGroupProfileAsync2(context.Message, cancellationToken));
            var response = await _groupService.GetGroupProfileAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<GroupDto>() { Data = response.Group };
        }
    }
}
