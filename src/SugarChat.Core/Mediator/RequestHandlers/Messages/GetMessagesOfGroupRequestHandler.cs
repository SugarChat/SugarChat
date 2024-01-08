using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Messages;
using SugarChat.Message.Requests;
using SugarChat.Message.Dtos;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Paging;
using SugarChat.Message.Basic;
using SugarChat.Core.Services;

namespace SugarChat.Core.Mediator.RequestHandlers.Messages
{
    public class GetMessagesOfGroupRequestHandler : IRequestHandler<GetMessagesOfGroupRequest, SugarChatResponse<PagedResult<MessageDto>>>
    {
        private readonly IMessageService _messageService;
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public GetMessagesOfGroupRequestHandler(IMessageService messageService, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _messageService = messageService;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }

        public async Task<SugarChatResponse<PagedResult<MessageDto>>> Handle(IReceiveContext<GetMessagesOfGroupRequest> context, CancellationToken cancellationToken)
        {
            _backgroundJobClientProvider.Enqueue(() => _messageService.GetMessagesOfGroupAsync2(context.Message, cancellationToken));
            var response = await _messageService.GetMessagesOfGroupAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<PagedResult<MessageDto>>() { Data = response.Messages };
        }
    }
}
