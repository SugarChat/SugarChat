using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Microsoft.Extensions.Logging;
using ServiceStack.Redis;
using SugarChat.Push.SignalR.Services;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Mediator.Goup
{
    public class GroupCommandHandler : ICommandHandler<GroupCommand>
    {
        private readonly IChatHubService _chatHubService;
        private readonly IRedisClient _redis;
        private readonly ILogger Logger;

        public GroupCommandHandler(IChatHubService chatHubService, IRedisClient redis, ILoggerFactory loggerFactory)
        {
            _chatHubService = chatHubService;
            _redis = redis;
            Logger = loggerFactory.CreateLogger<GroupCommandHandler>();
        }

        public async Task Handle(IReceiveContext<GroupCommand> context, CancellationToken cancellationToken)
        {
            var message = context.Message;
            var userConnectionIds = JsonSerializer.Deserialize<List<string>>(_redis.GetValueFromHash("UserConnectionIds", message.UserIdentifier));
            switch (message.Action)
            {
                case Enums.GroupAction.Add:
                    foreach (var userConnectionId in userConnectionIds)
                    {
                        await _chatHubService.AddGroups(userConnectionId, message.GroupNames, cancellationToken).ConfigureAwait(false);
                    }
                    break;
                case Enums.GroupAction.Exit:
                    foreach (var userConnectionId in userConnectionIds)
                    {
                        await _chatHubService.ExitGroups(userConnectionId, message.GroupNames, cancellationToken).ConfigureAwait(false);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
