using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;
using Microsoft.AspNetCore.Builder;
using SugarChat.Core.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Middlewares
{
    public static class MiddlewareExtension
    {
        public static IApplicationBuilder UseSugarException(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }

        public static void UnifyResponseMiddleware<TContext>(this IPipeConfigurator<TContext> configurator, Type unifiedType)
            where TContext : IContext<IMessage>
        {
            configurator.AddPipeSpecification(new UnifyResponseMiddlewareSpecification<TContext>(unifiedType));
        }
    }
}
