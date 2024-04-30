using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services;
using SugarChat.Core.Services.Messages;
using SugarChat.Message.Basic;
using SugarChat.Message.Requests.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Messages
{
    public class GetUnreadMessageCountRequestHandler : IRequestHandler<GetUnreadMessageCountRequest, SugarChatResponse<int>>
    {
        private readonly IMessageService _messageService;
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public GetUnreadMessageCountRequestHandler(IMessageService messageService, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _messageService = messageService;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }

        public async Task<SugarChatResponse<int>> Handle(IReceiveContext<GetUnreadMessageCountRequest> context, CancellationToken cancellationToken)
        {
            _backgroundJobClientProvider.Enqueue(() => _messageService.GetUnreadMessageCountAsync2(context.Message, cancellationToken));
            var response = await _messageService.GetUnreadMessageCountAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<int>() { Data = response.Count };
        }
    }
}
