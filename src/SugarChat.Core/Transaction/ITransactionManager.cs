using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Transaction
{
    public interface ITransactionManager
    {
        void BeginTransaction();

        Task CommitTransactionAsync();

        Task AbortTransactionAsync();
    }
}
