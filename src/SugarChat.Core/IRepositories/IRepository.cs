using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SugarChat.Core.Domain;
using SugarChat.Core.Services;
using SugarChat.Message.Paging;

namespace SugarChat.Core.IRepositories
{
    public interface IRepository
    {
        Task<List<T>> ToListAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : class, IEntity;
        Task<List<T>> ToListAsync<T>(IAsyncCursorSource<T> source, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IEntity;
        Task<PagedResult<T>> ToPagedListAsync<T>(PageSettings pageSettings, Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : class, IEntity;
        Task<PagedResult<T>> ToPagedListAsync<T>(PageSettings pageSettings, IQueryable<T> query = null, CancellationToken cancellationToken = default) where T : class, IEntity;
        Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : class, IEntity;
        Task<T> SingleOrDefaultAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : class, IEntity;
        Task<T> SingleAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : class, IEntity;
        Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : class, IEntity;
        Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : class, IEntity;
        IQueryable<T> Query<T>() where T : class, IEntity;
        Task<int> AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IEntity;
        Task<int> AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class, IEntity;
        Task<int> RemoveAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IEntity;
        Task<int> RemoveRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class, IEntity;
        Task<int> UpdateAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IEntity;
        Task<int> UpdateRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class, IEntity;
        Task<IAsyncCursor<BsonDocument>> GetAggregate<T>(IEnumerable<string> stages, CancellationToken cancellationToken = default) where T : class, IEntity;
        Task<IEnumerable<TDestination>> GetList<TSource, TDestination>(IEnumerable<string> stages, CancellationToken cancellationToken = default) where TSource : class, IEntity where TDestination : class;
    }
}
