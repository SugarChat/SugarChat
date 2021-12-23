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

namespace SugarChat.Core.Middlewares
{
    public static class UserExistMiddleware
    {
        public static void UseNeedUserExist<TContext>(this IPipeConfigurator<TContext> configurator) where TContext : IContext<IMessage>
        {
            var userDataProvider = configurator.DependencyScope.Resolve<IUserDataProvider>();
            var memoryCache = configurator.DependencyScope.Resolve<IMemoryCache>();
            var runtimeProvider = configurator.DependencyScope.Resolve<RunTimeProvider>();
            configurator.AddPipeSpecification(new UserExistSpecification<TContext>(userDataProvider, memoryCache, runtimeProvider));
        }
    }
}
