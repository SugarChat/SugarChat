using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Autofac;
using Mediator.Net;
using Mediator.Net.Autofac;
using Module = Autofac.Module;

namespace SugarChat.Core.Autofac
{
    public class SugarChatModule : Module
    {
        private readonly IEnumerable<Assembly> _assemblies;

        public SugarChatModule(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterMediator(builder);
        }

        private void RegisterMediator(ContainerBuilder builder)
        {
            var mediaBuilder = new MediatorBuilder();
            
            mediaBuilder.RegisterHandlers(_assemblies.ToArray());
            
            builder.RegisterMediator(mediaBuilder);
        }
    }
}