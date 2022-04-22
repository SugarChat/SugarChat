using SugarChat.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Transaction
{
    public class TransactionManager : ITransactionManager
    {
        readonly IRepository _repository;

        public TransactionManager(IRepository repository)
        {
            _repository = repository;
        }

        public ITransactionManager BeginTransaction()
        {
            _repository.BeginTransaction();
            return this;
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _repository.CommitTransactionAsync(cancellationToken);
        }

        public async Task AbortTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _repository.AbortTransactionAsync(cancellationToken);
        }

        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}
