using System;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.IRepositories
{
    public interface ITransactionManagement
    {
        ITransaction BeginTransaction();
        Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken);
    }
}
