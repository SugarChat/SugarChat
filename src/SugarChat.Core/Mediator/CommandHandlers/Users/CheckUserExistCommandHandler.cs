using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Users
{
    public class CheckUserExistCommandHandler : ICommandHandler<CheckUserExistCommand, SugarChatResponse<bool>>
    {
        private readonly IUserService _userService;

        public CheckUserExistCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<SugarChatResponse<bool>> Handle(IReceiveContext<CheckUserExistCommand> context, CancellationToken cancellationToken)
        {
            return new SugarChatResponse<bool>()
            {
                Data = await _userService.CheckUserExistAsync(context.Message, cancellationToken).ConfigureAwait(false)
            };
        }
    }
}
