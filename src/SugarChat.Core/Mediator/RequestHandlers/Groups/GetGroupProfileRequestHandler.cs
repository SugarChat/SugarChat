using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Groups;
using SugarChat.Message.Requests.Groups;
using SugarChat.Message.Responses.Groups;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Groups
{
    public class GetGroupProfileRequestHandler : IRequestHandler<GetGroupProfileRequest, GetGroupProfileResponse>
    {
        private readonly IGroupService _groupService;

        public GetGroupProfileRequestHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }
        public async Task<GetGroupProfileResponse> Handle(IReceiveContext<GetGroupProfileRequest> context, CancellationToken cancellationToken)
        {
            return await _groupService.GetGroupProfileAsync(context.Message, cancellationToken).ConfigureAwait(false);
        }
    }
}
