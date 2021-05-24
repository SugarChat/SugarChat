using Autofac;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SugarChat.Core.Autofac;
using SugarChat.Core.Services;
using SugarChat.Data.MongoDb.Autofac;
using System;
using System.Reflection;

namespace SugarChat.IntegrationTest
{
    public abstract class TestBase
    {
        protected readonly IConfiguration _configuration;
        protected ILifetimeScope Container { get; set; }

        protected TestBase()
        {
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var containerBuilder = new ContainerBuilder();
            RegisterBaseContainer(containerBuilder);
            Container = containerBuilder.Build();
        }

        private void RegisterBaseContainer(ContainerBuilder builder)
        {
            builder.RegisterMongoDbRepository(() => _configuration.GetSection("MongoDb"));
            builder.RegisterModule(new SugarChatModule(new Assembly[]
            {
                typeof(IService).Assembly
            }));
        }

       
    }
}