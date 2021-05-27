using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Microsoft.Extensions.Logging;
using SugarChat.Push.SignalR.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Mediator.SendMessage
{
    public class SendMessageCommandHandler : ICommandHandler<SendMessageCommand>
    {
        private readonly IChatHubService _chatHubService;
        private readonly ILogger Logger;

        public SendMessageCommandHandler(IChatHubService chatHubService, ILoggerFactory loggerFactory)
        {
            _chatHubService = chatHubService;
            Logger = loggerFactory.CreateLogger<SendMessageCommandHandler>();
        }

        public async Task Handle(IReceiveContext<SendMessageCommand> context, CancellationToken cancellationToken)
        {
            var message = context.Message;
            switch (message.Method)
            {
                case "SendUserMessage":
                    await _chatHubService.SendUserMessage(message.SendTo, message.Messages).ConfigureAwait(false);
                    break;
                case "SendMassUserMessage":
                    await _chatHubService.SendMassUserMessage(message.SendTos, message.Messages).ConfigureAwait(false);
                    break;
                case "SendGroupMessage":
                    await _chatHubService.SendGroupMessage(message.SendTo, message.Messages).ConfigureAwait(false);
                    break;
                case "SendMassGroupMessage":
                    await _chatHubService.SendMassGroupMessage(message.SendTos, message.Messages).ConfigureAwait(false);
                    break;
                case "SendAllMessage":
                    await _chatHubService.SendAllMessage(message.Messages).ConfigureAwait(false);
                    break;
                default:
                    if (message.IsMass)
                    {
                        await _chatHubService.CustomMassMessage(message.SendWay, message.Method, message.Messages, message.SendTos).ConfigureAwait(false);
                    }
                    else
                    {
                        await _chatHubService.CustomMessage(message.SendWay, message.Method, message.Messages, message.SendTo).ConfigureAwait(false);
                    }
                    break;
            }
        }
    }
}
