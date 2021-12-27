using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.UnitTest.Command;
using SugarChat.Message.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.UnitTest.Request
{
    public class TestDataFromResponseRequestHandler : IRequestHandler<TestRequest, SugarChatResponse<string>>
    {
        public Task<SugarChatResponse<string>> Handle(IReceiveContext<TestRequest> context, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SugarChatResponse<string>() { Data = "TestData" });
        }
    }
}
