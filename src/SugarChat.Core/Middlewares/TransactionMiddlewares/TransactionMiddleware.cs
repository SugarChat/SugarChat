using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;
using SugarChat.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Middlewares.TransactionMiddlewares
{
    public static class TransactionMiddleware
    {
        public static void UseTransaction<TContext>(this IPipeConfigurator<TContext> configurator) where TContext : IContext<IMessage>
        {
            var repository = configurator.DependencyScope.Resolve<IRepository>();
            configurator.AddPipeSpecification(new TransactionSpecification<TContext>(repository));
        }
    }
}
