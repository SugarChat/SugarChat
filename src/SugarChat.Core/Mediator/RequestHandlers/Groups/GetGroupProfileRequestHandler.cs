using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Groups;
using SugarChat.Message.Requests.Groups;
using SugarChat.Shared.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Groups
{
    public class GetGroupProfileRequestHandler : IRequestHandler<GetGroupProfileRequest, SugarChatResponse<GroupDto>>
    {
        private readonly IGroupService _groupService;

        public GetGroupProfileRequestHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }
        public async Task<SugarChatResponse<GroupDto>> Handle(IReceiveContext<GetGroupProfileRequest> context, CancellationToken cancellationToken)
        {
            var response = await _groupService.GetGroupProfileAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<GroupDto>() { Code = StatusCode.Ok, Message = "Success", Data = response.Result };
        }
    }
}
