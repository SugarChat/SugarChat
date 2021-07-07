using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Messages;
using SugarChat.Message.Requests;
using SugarChat.Message.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Messages
{
    public class GetMessagesOfGroupRequestHandler : IRequestHandler<GetMessagesOfGroupRequest, SugarChatResponse<IEnumerable<MessageDto>>>
    {
        private readonly IMessageService _messageService;
        public GetMessagesOfGroupRequestHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<SugarChatResponse<IEnumerable<MessageDto>>> Handle(IReceiveContext<GetMessagesOfGroupRequest> context, CancellationToken cancellationToken)
        {
            var response = await _messageService.GetMessagesOfGroupAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<IEnumerable<MessageDto>>() { Data = response.Messages };
        }
    }
}
