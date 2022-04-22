using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using SugarChat.Core.IRepositories;

namespace SugarChat.Data.MongoDb
{
    public class MongoDbTransactionManagement : ITransactionManagement
    {
        private readonly IDatabaseManagement _databaseManagement;
        public MongoDbTransactionManagement(IDatabaseManagement databaseManagement)
        {
            _databaseManagement = databaseManagement;
        }

        public ITransaction BeginTransaction()
        {
            _databaseManagement.IsBeginTransaction = true;
            _databaseManagement.Session.StartTransaction(new TransactionOptions(
                readConcern: ReadConcern.Snapshot,
                writeConcern: WriteConcern.WMajority,
                readPreference: ReadPreference.Primary));
            return new MongoDbTransaction(_databaseManagement);
        }

        public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            _databaseManagement.IsBeginTransaction = true;
            _databaseManagement.Session.StartTransaction(new TransactionOptions(
                readConcern: ReadConcern.Snapshot,
                writeConcern: WriteConcern.WMajority,
                readPreference: ReadPreference.Primary));
            return await Task.FromResult(new MongoDbTransaction(_databaseManagement));
        }
    }
}
