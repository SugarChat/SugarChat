using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;
using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Middlewares.EventPublisher
{
    public class EventPublisherMiddlewareSpecification<TContext> : IPipeSpecification<TContext>
        where TContext : IContext<IMessage>
    {
        public Task AfterExecute(TContext context, CancellationToken cancellationToken)
        {
            return Task.WhenAll();
        }

        public Task BeforeExecute(TContext context, CancellationToken cancellationToken)
        {
            return Task.WhenAll();
        }

        public Task Execute(TContext context, CancellationToken cancellationToken)
        {
            if (ShouldExecute(context, cancellationToken))
            {
            }
            return Task.WhenAll();
        }

        public Task OnException(Exception ex, TContext context)
        {
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw ex;
        }

        public bool ShouldExecute(TContext context, CancellationToken cancellationToken)
        {
            return true;
        }
    }
}
