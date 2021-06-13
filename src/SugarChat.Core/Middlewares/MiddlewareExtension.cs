using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;

namespace SugarChat.Core.Middlewares
{
    public static class MiddlewareExtension
    {
        public static void AddPipeSpecifications<TContext>(this IPipeConfigurator<TContext> configurator)
            where TContext : IContext<IMessage>
        {
            configurator.AddPipeSpecification(new UnifyResponseMiddlewareSpecification<TContext>());
            configurator.AddPipeSpecification(new ValidatorSpecification<TContext>());
            
        }
    }
}
