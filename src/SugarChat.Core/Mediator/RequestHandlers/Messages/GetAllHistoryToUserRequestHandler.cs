﻿using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Messages;
using SugarChat.Message.Requests;
using SugarChat.Message.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Basic;

namespace SugarChat.Core.Mediator.RequestHandlers.Messages
{
    public class GetAllHistoryToUserRequestHandler : IRequestHandler<GetAllHistoryToUserRequest, SugarChatResponse<IEnumerable<MessageDto>>>
    {
        private readonly IMessageService _messageService;
        public GetAllHistoryToUserRequestHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<SugarChatResponse<IEnumerable<MessageDto>>> Handle(IReceiveContext<GetAllHistoryToUserRequest> context, CancellationToken cancellationToken)
        {
            var response = await _messageService.GetAllHistoryToUserAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<IEnumerable<MessageDto>>() { Data = response.Messages };
        }
    }
}
