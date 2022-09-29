using Mediator.Net.Context;
using Mediator.Net.Contracts;
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
    public class JudgeUserInGroupCommandHandler : ICommandHandler<JudgeUserInGroupCommand, SugarChatResponse<bool>>
    {
        private readonly IGroupUserService _groupUserService;
        public JudgeUserInGroupCommandHandler(IGroupUserService groupUserService)
        {
            _groupUserService = groupUserService;
        }
        public async Task<SugarChatResponse<bool>> Handle(IReceiveContext<JudgeUserInGroupCommand> context, CancellationToken cancellationToken)
        {
            return new SugarChatResponse<bool>()
            {
                Data = await _groupUserService.JudgeUserInGroupAsync(context.Message, cancellationToken).ConfigureAwait(false)
            };
        }
    }
}
