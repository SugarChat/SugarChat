using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SugarChat.Core.IRepositories
{
    public interface IRepositoryBase<T>
    {
        Task AddAsync(T user);
        Task DeleteAsync(Guid userId);
        Task<T> FindAsync(Guid userId);
        Task UpdateAsync(T newMessage, Expression<Func<T, object>> updatedFields);
    }
}
