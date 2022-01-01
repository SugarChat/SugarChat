using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Middlewares
{
    public class RequestOverallElapsedSpecification<TContext> : IPipeSpecification<TContext>
            where TContext : IContext<IMessage>
    {
        private readonly ILogger _logger;

        private readonly Stack<Stopwatch> _stopwatchStack = new Stack<Stopwatch>(4);

        public RequestOverallElapsedSpecification(ILogger logger)
        {
            _logger = logger;
        }

        public bool ShouldExecute(TContext context, CancellationToken cancellationToken)
        {
            return true;
        }

        public Task BeforeExecute(TContext context, CancellationToken cancellationToken)
        {
            _stopwatchStack.Push(Stopwatch.StartNew());
            return Task.WhenAll();
        }

        public Task Execute(TContext context, CancellationToken cancellationToken)
        {
            return Task.WhenAll();
        }

        public Task AfterExecute(TContext context, CancellationToken cancellationToken)
        {
            if (_stopwatchStack.Count > 0)
            {
                _logger.Information("{MessageName} Overall Elapsed:{Ms}", context.Message?.GetType()?.Name, _stopwatchStack.Pop().ElapsedMilliseconds);
            }

            return Task.WhenAll();
        }

        public Task OnException(Exception ex, TContext context)
        {
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw ex;
        }
    }
}
