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
using System;

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
            RegisterDataProviders(builder);
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

            if (_assemblies == null)
                return;
            var profileTypes = new List<Type>();
            profileTypes = _assemblies.Aggregate(profileTypes,
                (acc, a) =>
                {
                    return acc.Concat(a.GetTypes().Where(t => typeof(Profile).IsAssignableFrom(t) && !t.IsAbstract && t.IsPublic)).ToList();
                });

            builder.RegisterTypes(profileTypes.ToArray()).AsSelf().As<Profile>();

            builder.Register(c => new MapperConfiguration(cfg =>
              {
                  var profiles = c.Resolve<IEnumerable<Profile>>().ToList();
                  profiles.ForEach(x => { cfg.AddProfile(x); });

              })).AsSelf().SingleInstance();                  
        }

        private void RegisterServices(ContainerBuilder builder)
        {
            foreach (var type in typeof(IService).GetTypeInfo().Assembly.GetTypes()
                .Where(t => typeof(IService).IsAssignableFrom(t) && t.IsClass))
            {
                builder.RegisterType(type).AsSelf().AsImplementedInterfaces();
            }
        }

        private void RegisterDataProviders(ContainerBuilder builder)
        {
            foreach (var type in typeof(IDataProvider).GetTypeInfo().Assembly.GetTypes()
                .Where(t => typeof(IDataProvider).IsAssignableFrom(t) && t.IsClass))
            {
                builder.RegisterType(type).AsSelf().AsImplementedInterfaces();
            }
        }
    }
}