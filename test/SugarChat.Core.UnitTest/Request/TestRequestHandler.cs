﻿using System.Threading;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Exceptions;
using SugarChat.Core.UnitTest.Command;

namespace SugarChat.Core.UnitTest.Request
{
    public class TestRequestHandler : IRequestHandler<TestRequest, TestRequestResponse>
    {
        public Task<TestRequestResponse> Handle(IReceiveContext<TestRequest> context, CancellationToken cancellationToken)
        {
            throw new BusinessWarningException("Test");
        }
    }
}
