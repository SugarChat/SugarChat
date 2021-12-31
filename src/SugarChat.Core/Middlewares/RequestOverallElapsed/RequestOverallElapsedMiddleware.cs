using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;
using Serilog;

namespace SugarChat.Core.Middlewares.RequestOverallElapsed
{
    public static class RequestOverallElapsedMiddleware
    {
        public static void UseLogOverallElapsed<TContext>(this IPipeConfigurator<TContext> configurator,
            ILogger logger) where TContext : IContext<IMessage>
        {
            configurator.AddPipeSpecification(new RequestOverallElapsedSpecification<TContext>(logger));
        }
    }
}