using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.UnitTest.Command
{
    public class TestRequestHandler : IRequestHandler<TestRequest, TestRequestResponse>
    {
        public Task<TestRequestResponse> Handle(IReceiveContext<TestRequest> context, CancellationToken cancellationToken)
        {
            throw new BusinessWarningException("Test");
        }
    }
}
