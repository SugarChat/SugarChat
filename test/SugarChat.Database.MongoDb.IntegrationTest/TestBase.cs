using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using System;
using System.Configuration;

namespace SugarChat.Database.MongoDb.IntegrationTest
{
    public abstract class TestBase
    {
        protected readonly IConfiguration _configuration;

        protected TestBase()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }
    }
}