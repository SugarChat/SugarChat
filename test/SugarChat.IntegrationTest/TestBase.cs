using Autofac;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SugarChat.Core.Autofac;
using SugarChat.Data.MongoDb.Autofac;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using SugarChat.SignalR.ServerClient;
using Xunit;
using NSubstitute;
using SugarChat.Core.Services;
using Microsoft.Extensions.Caching.Memory;
using SugarChat.Core;
using SugarChat.Message;
using SugarChat.Core.Utils;
using System.Linq.Expressions;
using SugarChat.Message.Exceptions;
using Newtonsoft.Json;

namespace SugarChat.IntegrationTest
{
    [Collection("tests")]
    public abstract class TestBase : IDisposable
    {
        protected IConfiguration _configuration;
        protected ILifetimeScope Container { get; set; }

        protected TestBase()
        {
            LoadThisConfiguration();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterMongoDbRepository(() => _configuration.GetSection("MongoDb"));
            containerBuilder.RegisterInstance(_configuration).As<IConfiguration>();
            containerBuilder.RegisterType<SignalRClientMock>()
                .As<IServerClient>()
                .InstancePerLifetimeScope();

            MemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            containerBuilder.RegisterInstance(memoryCache).AsImplementedInterfaces();

            RegisterBaseContainer(containerBuilder, builder =>
            {
                var iSecurityManager = Substitute.For<ISecurityManager>();
                iSecurityManager.IsSupperAdmin().Returns(false);
                containerBuilder.RegisterInstance(iSecurityManager);
            });
            RegisterBackgroundJobClientProvider(containerBuilder);
            containerBuilder.RegisterType<TableUtil>().As<ITableUtil>().InstancePerLifetimeScope();

            Container = containerBuilder.Build().BeginLifetimeScope();
        }

        private void LoadThisConfiguration()
        {
            _configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();
        }

        private void RegisterBaseContainer(ContainerBuilder builder, Action<ContainerBuilder> extraRegistration)
        {
            builder.RegisterModule(new SugarChatModule(new Assembly[]
            {
                typeof(SugarChat.Core.Services.IService).Assembly
            },
            new RunTimeProvider(RunTimeType.Test)));
            extraRegistration(builder);
        }

        private void RegisterBackgroundJobClientProvider(ContainerBuilder builder)
        {
            var backgroundJobClientProvider = Substitute.For<IBackgroundJobClientProvider>();
            backgroundJobClientProvider.Enqueue(Arg.Any<Expression<Func<Task>>>()).Returns(x =>
            {
                try
                {
                    var call = (Expression<Func<Task>>)x.Args()[0];
                    var func = call.Compile();
                    func().Wait();
                }
                catch (Exception ex)
                {
                    if (!JsonConvert.SerializeObject(ex.InnerException).Contains("\"LogLevel\":3"))
                    {
                        throw;
                    }
                }
                return default;
            });
            builder.RegisterInstance(backgroundJobClientProvider).SingleInstance();
        }

        protected void Run<T>(Action<T> action, Action<ContainerBuilder> extraRegistration = null)
        {
            var dependency = extraRegistration != null
                ? Container.BeginLifetimeScope(extraRegistration).Resolve<T>()
                : Container.BeginLifetimeScope().Resolve<T>();
            action(dependency);
        }

        protected void Run<T, R>(Action<T, R> action, Action<ContainerBuilder> extraRegistration = null)
        {
            var lifetime = extraRegistration != null
                ? Container.BeginLifetimeScope(extraRegistration)
                : Container.BeginLifetimeScope();

            var dependency = lifetime.Resolve<T>();
            var dependency2 = lifetime.Resolve<R>();
            action(dependency, dependency2);
        }

        protected async Task Run<T, R, L>(Func<T, R, L, Task> action, Action<ContainerBuilder> extraRegistration = null)
        {
            var lifetime = extraRegistration != null
                ? Container.BeginLifetimeScope(extraRegistration)
                : Container.BeginLifetimeScope();

            var dependency = lifetime.Resolve<T>();
            var dependency2 = lifetime.Resolve<R>();
            var dependency3 = lifetime.Resolve<L>();
            await action(dependency, dependency2, dependency3);
        }

        protected async Task Run<T>(Func<T, Task> action, Action<ContainerBuilder> extraRegistration = null)
        {
            var dependency = extraRegistration != null
                ? Container.BeginLifetimeScope(extraRegistration).Resolve<T>()
                : Container.BeginLifetimeScope().Resolve<T>();
            await action(dependency);
        }

        protected async Task<R> Run<T, R>(Func<T, Task<R>> action, Action<ContainerBuilder> extraRegistration = null)
        {
            var dependency = extraRegistration != null
                ? Container.BeginLifetimeScope(extraRegistration).Resolve<T>()
                : Container.BeginLifetimeScope().Resolve<T>();
            return await action(dependency);
        }

        protected R Run<T, R>(Func<T, R> action, Action<ContainerBuilder> extraRegistration = null)
        {
            var dependency = extraRegistration != null
                ? Container.BeginLifetimeScope(extraRegistration).Resolve<T>()
                : Container.BeginLifetimeScope().Resolve<T>();
            return action(dependency);
        }

        protected R Run<T, U, R>(Func<T, U, R> action, Action<ContainerBuilder> extraRegistration = null)
        {
            var lifetime = extraRegistration != null
                ? Container.BeginLifetimeScope(extraRegistration)
                : Container.BeginLifetimeScope();

            var dependency = lifetime.Resolve<T>();
            var dependency2 = lifetime.Resolve<U>();
            return action(dependency, dependency2);
        }

        protected Task Run<T, U>(Func<T, U, Task> action, Action<ContainerBuilder> extraRegistration = null)
        {
            var lifetime = extraRegistration != null
                ? Container.BeginLifetimeScope(extraRegistration)
                : Container.BeginLifetimeScope();
            var dependency = lifetime.Resolve<T>();
            var dependency2 = lifetime.Resolve<U>();
            return action(dependency, dependency2);
        }

        public void Dispose()
        {
            ClearDatabaseRecord();
        }

        private static readonly object databaseLock = new object();

        private void ClearDatabaseRecord()
        {
            lock (databaseLock)
            {
                var client = new MongoClient(_configuration["MongoDb:ConnectionString"]);
                client.DropDatabase(_configuration["MongoDb:DatabaseName"]);
            }
        }
    }
}