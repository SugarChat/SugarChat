using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Transaction
{
    public interface ITransactionManager : IDisposable
    {
        ITransactionManager BeginTransaction();

        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        Task AbortTransactionAsync(CancellationToken cancellationToken = default);
    }
}
