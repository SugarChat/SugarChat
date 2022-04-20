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

        public void CommitTransaction()
        {
            _repository.CommitTransaction();
        }

        public void AbortTransaction()
        {
            _repository.AbortTransaction();
        }
    }
}
