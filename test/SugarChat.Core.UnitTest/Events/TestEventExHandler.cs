using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.UnitTest.Events
{
    public class TestEventExHandler : IEventHandler<TestEvent>
    {
        public Task Handle(IReceiveContext<TestEvent> context, CancellationToken cancellationToken)
        {
            throw new BusinessException("Test");
        }
    }
}
