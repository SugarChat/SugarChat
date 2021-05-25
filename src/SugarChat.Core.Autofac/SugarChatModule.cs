using Autofac;
using AutoMapper;
using Mediator.Net;
using Mediator.Net.Autofac;
using SugarChat.Core.Services;
using SugarChat.Message.Command;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Module = Autofac.Module;
using Mediator.Net.Binding;
using SugarChat.Core.Basic;
using SugarChat.Core.Middlewares;
using Mediator.Net.Context;
using Mediator.Net.Contracts;

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
            RegisterServices(builder);
            RegisterAutoMapper(builder);
        }

        private void RegisterMediator(ContainerBuilder builder)
        {
            var mediaBuilder = new MediatorBuilder();

            mediaBuilder
                .ConfigureGlobalReceivePipe(config => config.UnifyResponseMiddleware(typeof(ISugarChatResponse)))
                .RegisterHandlers(_assemblies.ToArray());
            
            builder.RegisterMediator(mediaBuilder);
        }


        private void RegisterAutoMapper(ContainerBuilder builder)
        {
            builder.Register(context => new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SendMessageCommand, Domain.Message>();
            }))
            .AsSelf()
            .SingleInstance();

            builder.Register(c =>
            {
                var context = c.Resolve<IComponentContext>();
                var config = context.Resolve<MapperConfiguration>();
                return config.CreateMapper(context.Resolve);
            })
            .As<IMapper>()
            .InstancePerLifetimeScope();
        }

        private void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<SendMessageService>()
                .As<ISendMessageService>()
                .InstancePerLifetimeScope();
        }
    }
}