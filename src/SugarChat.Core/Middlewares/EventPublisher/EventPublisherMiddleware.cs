using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;
using Microsoft.Extensions.Caching.Memory;
using SugarChat.Core.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Middlewares.EventPublisher
{
    public static class EventPublisherMiddleware
    {
        public static void UseEventPublisher<TContext>(this IPipeConfigurator<TContext> configurator) where TContext : IContext<IMessage>
        {
            configurator.AddPipeSpecification(new EventPublisherMiddlewareSpecification<TContext>());
        }
    }
}
