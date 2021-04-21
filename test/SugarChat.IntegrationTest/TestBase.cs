using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SugarChat.Core.Tools;
using System;

namespace SugarChat.IntegrationTest
{
    public abstract class TestBase
    {
        protected readonly IConfiguration _configuration;

        protected TestBase()
        {
            BsonSerializer.RegisterSerializationProvider(new LocalDateTimeSerializationProvider());
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }
    }
}