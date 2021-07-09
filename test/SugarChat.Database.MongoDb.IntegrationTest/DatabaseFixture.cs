using System;
using System.IO;
using Autofac;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using NSubstitute;
using SugarChat.Core.Autofac;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Services;
using SugarChat.Data.MongoDb.Autofac;

namespace SugarChat.Database.MongoDb.IntegrationTest
{
    public class DatabaseFixture : IDisposable
    {
        public readonly IConfigurationRoot Configuration;
        public readonly IMongoClient Client;
        public ILifetimeScope Container;
        public readonly IRepository Repository;

        public DatabaseFixture()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var containerBuilder = new ContainerBuilder();
            containerBuilder
                .RegisterMongoDbRepository(() => Configuration.GetSection("MongoDb"))
                .RegisterModule(new SugarChatModule(new[]
                {
                    typeof(SugarChat.Core.Services.IService).Assembly
                }));

            var iSecurityManager = Substitute.For<ISecurityManager>();
            iSecurityManager.IsSupperAdmin().Returns(false);
            containerBuilder.RegisterInstance(iSecurityManager);

            Container = containerBuilder.Build().BeginLifetimeScope();
            Repository = Container.Resolve<IRepository>();
            Client = new MongoClient(Configuration["MongoDb:ConnectionString"]);
        }

        public void Dispose()
        {
        }
    }
}