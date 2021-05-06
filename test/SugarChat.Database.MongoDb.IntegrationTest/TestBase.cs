using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using SugarChat.Core.Tools;
using System;
using System.Configuration;

namespace SugarChat.Database.MongoDb.IntegrationTest
{
    public abstract class TestBase
    {
        protected readonly IConfiguration _configuration;

        protected TestBase()
        {
            BsonSerializer.RegisterSerializationProvider(new LocalDateTimeSerializationProvider());
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }
    }
}