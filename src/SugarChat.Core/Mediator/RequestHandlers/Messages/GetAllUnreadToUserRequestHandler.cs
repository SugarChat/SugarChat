using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Messages;
using SugarChat.Message.Requests;
using SugarChat.Shared.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Messages
{
    public class GetAllUnreadToUserRequestHandler : IRequestHandler<GetAllUnreadToUserRequest, SugarChatResponse<IEnumerable<MessageDto>>>
    {
        private readonly IMessageService _messageService;
        public GetAllUnreadToUserRequestHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<SugarChatResponse<IEnumerable<MessageDto>>> Handle(IReceiveContext<GetAllUnreadToUserRequest> context, CancellationToken cancellationToken)
        {
            var response = await _messageService.GetAllUnreadToUserAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<IEnumerable<MessageDto>>() { Data = response.Messages };
        }
    }
}
