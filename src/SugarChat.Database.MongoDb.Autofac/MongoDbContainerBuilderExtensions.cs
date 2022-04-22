using Autofac;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using SugarChat.Core.IRepositories;
using SugarChat.Data.MongoDb.Settings;
using SugarChat.Data.MongoDb.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace SugarChat.Data.MongoDb.Autofac
{
    public static class MongoDbContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterMongoDbRepository(this ContainerBuilder builder, Func<IConfiguration> setConfiguration)
        {
            var configuration = setConfiguration?.Invoke();
            if (configuration == null)
            {
                throw new Exception(string.Format("The delegate {0} returns null", nameof(setConfiguration)));
            }

            var settings = configuration.Get<MongoDbSettings>();

            if (settings == null)
            {
                throw new Exception("Incorrect configuration section");
            }

            builder.RegisterInstance(settings)
                   .SingleInstance();

            builder.RegisterType<MongoDbRepository>()
                   .As<IRepository>()
                   .InstancePerLifetimeScope();


            builder.RegisterType<DatabaseManagement>()
                .As<IDatabaseManagement>()
                .InstancePerLifetimeScope();

            return builder;
        }
    }
}
