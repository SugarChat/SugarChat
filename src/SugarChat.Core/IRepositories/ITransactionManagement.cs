using System;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.IRepositories
{
    public interface ITransactionManagement : IDisposable
    {
        bool IsBeginTransaction { get; set; }
        void Commit();
        Task CommitAsync(CancellationToken cancellationToken);
        void Rollback();
        Task RollbackAsync(CancellationToken cancellationToken);

    }
}
