﻿using Mediator.Net.Context;
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
    public class CheckUserIdsInGroupCommandHandler : ICommandHandler<CheckUserIdsInGroupCommand, SugarChatResponse<bool>>
    {
        private readonly IGroupUserService _groupUserService;
        public CheckUserIdsInGroupCommandHandler(IGroupUserService groupUserService)
        {
            _groupUserService = groupUserService;
        }
        public async Task<SugarChatResponse<bool>> Handle(IReceiveContext<CheckUserIdsInGroupCommand> context, CancellationToken cancellationToken)
        {
            return new SugarChatResponse<bool>()
            {
                Data = await _groupUserService.CheckUserIdsInGroupAsync(context.Message, cancellationToken).ConfigureAwait(false)
            };
        }
    }
}
