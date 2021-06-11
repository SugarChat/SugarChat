using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Messages;
using SugarChat.Message.Requests.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Messages
{
    public class GetUnreadMessageCountRequestHandler : IRequestHandler<GetUnreadMessageCountRequest, SugarChatResponse<int>>
    {
        private readonly IMessageService _messageService;
        public GetUnreadMessageCountRequestHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<SugarChatResponse<int>> Handle(IReceiveContext<GetUnreadMessageCountRequest> context, CancellationToken cancellationToken)
        {
            var response = await _messageService.GetUnreadMessageCountAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<int>() { Data = response.Count };
        }
    }
}
