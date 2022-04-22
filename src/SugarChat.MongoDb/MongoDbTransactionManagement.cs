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

        public void Dispose()
        {
            _databaseManagement.DisposeSession();
            _databaseManagement.IsBeginTransaction = false;
        }

        public bool IsBeginTransaction { get; set; }

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            if (_databaseManagement.Session == null || !_databaseManagement.Session.IsInTransaction)
                throw new Exception("mongodb transaction is not ready");
            await _databaseManagement.Session.CommitTransactionAsync(cancellationToken);
        }

        public async Task RollbackAsync(CancellationToken cancellationToken)
        {
            if (_databaseManagement.Session == null || !_databaseManagement.Session.IsInTransaction)
                throw new Exception("mongodb transaction is not ready");
            await _databaseManagement.Session.AbortTransactionAsync(cancellationToken);
        }

        public void Commit()
        {
            if (_databaseManagement.Session == null || !_databaseManagement.Session.IsInTransaction)
                throw new Exception("mongodb transaction is not ready");
            _databaseManagement.Session.CommitTransaction();
        }

        public void Rollback()
        {
            if (_databaseManagement.Session == null || !_databaseManagement.Session.IsInTransaction)
                throw new Exception("mongodb transaction is not ready");
            _databaseManagement.Session.AbortTransaction();
        }
    }
}
