using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using SugarChat.Data.MongoDb.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Data.MongoDb
{
    public class MongoDbClient : MongoClient
    {
        public MongoDbClient(MongoDbSettings connectionString) :base(connectionString.ConnectionString)
        {

        }
    }
}
