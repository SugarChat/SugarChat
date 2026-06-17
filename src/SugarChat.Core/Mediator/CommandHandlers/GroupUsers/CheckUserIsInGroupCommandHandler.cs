using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.GroupUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.GroupUsers
{
    public class CheckUserIsInGroupCommandHandler : ICommandHandler<CheckUserIsInGroupCommand, SugarChatResponse<bool>>
    {
        private readonly IGroupUserService _groupUserService;
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public CheckUserIsInGroupCommandHandler(IGroupUserService groupUserService, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _groupUserService = groupUserService;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }
        public async Task<SugarChatResponse<bool>> Handle(IReceiveContext<CheckUserIsInGroupCommand> context, CancellationToken cancellationToken)
        {
            _backgroundJobClientProvider.Enqueue(() => _groupUserService.CheckUserIsInGroupAsync2(context.Message, cancellationToken));
            return new SugarChatResponse<bool>()
            {
                Data = await _groupUserService.CheckUserIsInGroupAsync(context.Message, cancellationToken).ConfigureAwait(false)
            };
        }
    }
}
