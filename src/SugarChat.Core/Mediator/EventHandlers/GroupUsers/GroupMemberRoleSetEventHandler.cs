﻿using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Events.GroupUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.EventHandlers.GroupUsers
{
    public class GroupMemberRoleSetEventHandler : IEventHandler<GroupMemberRoleSetEvent>
    {
        public Task Handle(IReceiveContext<GroupMemberRoleSetEvent> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
