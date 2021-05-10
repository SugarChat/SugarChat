using Autofac;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using SugarChat.Core.IRepositories;
using SugarChat.Data.MongoDb.Settings;
using SugarChat.Data.MongoDb.Tools;

namespace SugarChat.Data.MongoDb.Autofac
{
    public class MongoDbModule : Module
    {
        private readonly IConfiguration _configuration;
        public MongoDbModule(IConfiguration configuration)
        {
            _configuration = configuration;
            BsonSerializer.RegisterSerializationProvider(new LocalDateTimeSerializationProvider());
        }
        protected override void Load(ContainerBuilder builder)
        {
            var mongoSetings = _configuration.GetSection("MongoDb")
                                             .Get<MongoDbSettings>();
            builder.RegisterInstance(mongoSetings)
                   .SingleInstance();
            builder.RegisterType<MongoDbRepository>()
                   .As<IRepository>()
                   .SingleInstance();
        }
    }
}
