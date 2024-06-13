using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Microsoft.Extensions.Logging;
using SugarChat.Push.SignalR.Models;
using SugarChat.Push.SignalR.Services;
using SugarChat.Push.SignalR.Services.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Mediator.Goup
{
    public class GroupCommandHandler : ICommandHandler<GroupCommand>
    {
        private readonly IChatHubService _chatHubService;
        private readonly ILogger Logger;
        private readonly ICacheService _cacheService;

        public GroupCommandHandler(IChatHubService chatHubService, ILoggerFactory loggerFactory, ICacheService cacheService)
        {
            _chatHubService = chatHubService;
            Logger = loggerFactory.CreateLogger<GroupCommandHandler>();
            _cacheService = cacheService;
        }

        public async Task Handle(IReceiveContext<GroupCommand> context, CancellationToken cancellationToken)
        {
            var message = context.Message;
            var userConnectionIds = await _cacheService.GetHashByKeyFromRedis<List<string>>("UserConnectionIds", message.UserIdentifier);
            switch (message.Action)
            {
                case Enums.GroupAction.Add:
                    foreach (var userConnectionId in userConnectionIds)
                    {
                        await _chatHubService.AddGroup(userConnectionId, message.GroupName).ConfigureAwait(false);
                    }
                    break;
                case Enums.GroupAction.Exit:
                    foreach (var userConnectionId in userConnectionIds)
                    {
                        await _chatHubService.ExitGroup(userConnectionId, message.GroupName).ConfigureAwait(false);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
