using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using SugarChat.Core.IRepositories;
using SugarChat.Data.MongoDb.Settings;

namespace SugarChat.Data.MongoDb
{
    public interface IDatabaseManagement
    {
        IClientSessionHandle Session { get; }
        bool IsBeginTransaction { get; set; }
        IMongoDatabase GetDatabase();
        void DisposeSession();
        void StartSession();
    }
    public class DatabaseManagement : IDatabaseManagement
    {
        readonly IMongoClient _mongoClient;
        readonly MongoDbSettings _settings;

        private IClientSessionHandle _session;
        public IClientSessionHandle Session => _session;

        public bool IsBeginTransaction { get; set; }

        public DatabaseManagement(IMongoClient mongoClient, MongoDbSettings settings)
        {
            _mongoClient = mongoClient;
            _settings = settings;
        }

        private IMongoDatabase _database;

        public IMongoDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    _database = _mongoClient.GetDatabase(_settings.DatabaseName);
                }
                return _database;
            }
        }

        private IMongoDatabase _transactionDatabase;

        public IMongoDatabase TransactionDatabase
        {
            get
            {
                if (_transactionDatabase == null || _session == null)
                {
                    _transactionDatabase = _session.Client.GetDatabase(_settings.DatabaseName);
                }
                return _transactionDatabase;
            }
        }

        public IMongoDatabase GetDatabase()
        {
            if (IsBeginTransaction)
                return TransactionDatabase;
            else
                return Database;
        }

        public void DisposeSession()
        {
            _session.Dispose();
            _session = null;
        }

        public void StartSession()
        {
            _session = _mongoClient.StartSession();
            IsBeginTransaction = true;
        }
    }
}
