using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Messages;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Conversations
{
    public class GetAllToUserFromGroupRequestHandler : IRequestHandler<GetAllToUserFromGroupRequest, GetAllToUserFromGroupResponse>
    {
        private readonly IMessageService _messageService;

        public GetAllToUserFromGroupRequestHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }
        public async Task<GetAllToUserFromGroupResponse> Handle(IReceiveContext<GetAllToUserFromGroupRequest> context, CancellationToken cancellationToken)
        {
            return await _messageService.GetAllToUserFromGroupAsync(context.Message, cancellationToken);
        }
    }
}
