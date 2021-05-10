using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Autofac;
using Mediator.Net;
using Mediator.Net.Autofac;
using Module = Autofac.Module;
using Microsoft.Extensions.Configuration;

namespace SugarChat.Core.Autofac
{
    public class SugarChatModule : Module
    {
        private readonly IEnumerable<Assembly> _assemblies;
        private readonly IConfiguration _configuration;

        public SugarChatModule(IEnumerable<Assembly> assemblies, IConfiguration configuration)
        {
            _configuration = configuration;
            _assemblies = assemblies;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterMediator(builder);
            RegisterRepository(builder);
        }

        private void RegisterMediator(ContainerBuilder builder)
        {
            var mediaBuilder = new MediatorBuilder();

            mediaBuilder.RegisterHandlers(_assemblies.ToArray());

            builder.RegisterMediator(mediaBuilder);
        }

        private void RegisterRepository(ContainerBuilder builder)
        {
            builder.RegisterModule(new SugarChat.Data.MongoDb.Autofac.MongoDbModule(_configuration));
        }
    }
}