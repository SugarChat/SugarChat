using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;

namespace SugarChat.Core.Middlewares
{
    public static class MiddlewareExtension
    {
        public static void UseUnifyResponseMiddleware<TContext>(this IPipeConfigurator<TContext> configurator)
            where TContext : IContext<IMessage>
        {
            configurator.AddPipeSpecification(new UnifyResponseMiddlewareSpecification<TContext>());
        }
        
        public static void UseValidatorMiddleware<TContext>(this IPipeConfigurator<TContext> configurator)
            where TContext : IContext<IMessage>
        {
            configurator.AddPipeSpecification(new ValidatorMiddlewareSpecification<TContext>());
        }
    }
}
