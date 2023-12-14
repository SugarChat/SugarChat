using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Messages;
using SugarChat.Message.Requests.Messages;
using SugarChat.Message.Dtos;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Basic;
using SugarChat.Message.Paging;

namespace SugarChat.Core.Mediator.RequestHandlers.Messages
{
    public class GetMessagesByGroupIdsRequestHandler : IRequestHandler<GetMessagesByGroupIdsRequest, SugarChatResponse<PagedResult<MessageDto>>>
    {
        private readonly IMessageService _messageService;
        public GetMessagesByGroupIdsRequestHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<SugarChatResponse<PagedResult<MessageDto>>> Handle(IReceiveContext<GetMessagesByGroupIdsRequest> context, CancellationToken cancellationToken)
        {
            var response = await _messageService.GetMessagesByGroupIdsAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<PagedResult<MessageDto>>() { Data = response.Messages };
        }
    }
}
