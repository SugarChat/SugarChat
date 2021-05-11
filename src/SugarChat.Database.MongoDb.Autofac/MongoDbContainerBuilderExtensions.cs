using Autofac;
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
        public static ContainerBuilder RegisterMongoDbRepository(this ContainerBuilder builder, Func<MongoDbSettings> setSettings)
        {
            //BsonSerializer.RegisterSerializationProvider(new LocalDateTimeSerializationProvider());
            var settings = setSettings?.Invoke();
            builder.RegisterInstance(settings)
              .SingleInstance();
            builder.RegisterType<MongoDbRepository>()
              .As<IRepository>()
              .SingleInstance();
            return builder;
        }
    }
}
