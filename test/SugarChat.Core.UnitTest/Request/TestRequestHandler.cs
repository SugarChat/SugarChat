﻿using System.Threading;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Exceptions;
using SugarChat.Core.UnitTest.Command;
using SugarChat.Message.Basic;

namespace SugarChat.Core.UnitTest.Request
{
    public class TestRequestHandler : IRequestHandler<TestRequest, SugarChatResponse>
    {
        public Task<SugarChatResponse> Handle(IReceiveContext<TestRequest> context, CancellationToken cancellationToken)
        {
            throw new BusinessWarningException(Prompt.NotAllUsersExists);
        }
    }
}
