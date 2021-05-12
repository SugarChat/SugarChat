using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;

namespace SugarChat.Core.IRepositories
{
    public interface IRepository
    {
        Task<List<T>> ToListAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class, IEntity;
        Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class, IEntity;
        Task<T> SingleOrDefaultAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class, IEntity;
        Task<T> SingleAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class, IEntity;
        Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class, IEntity;
        Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class, IEntity;
        IQueryable<T> Query<T>(Expression<Func<T, bool>> predicate = null) where T : class, IEntity;
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IEntity;
        Task AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class, IEntity;
        Task RemoveAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IEntity;
        Task RemoveRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class, IEntity;
        Task UpdateAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IEntity;
        Task UpdateRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class, IEntity;
    }
}
