using SugarChat.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void BeginTransaction()
        {
            _repository.BeginTransaction();
        }

        public async Task CommitTransactionAsync()
        {
           await _repository.CommitTransactionAsync();
        }

        public async Task AbortTransactionAsync()
        {
            await _repository.AbortTransactionAsync();
        }
    }
}
