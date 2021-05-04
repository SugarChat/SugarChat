using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;

namespace SugarChat.Core.Services.Users
{
    public class Repository : IRepository
    {
        
        public Task<List<T>> ToListAsync<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity
        {
            throw new NotImplementedException();
        }

        public Task<T> SingleOrDefaultAsync<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity
        {
            throw new NotImplementedException();
        }

        public Task<T> SingleAsync<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity
        {
            throw new NotImplementedException();
        }

        public Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Query<T>(Expression<Func<T, bool>> predicate = null) where T : class, IEntity
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync<T>(T entity, CancellationToken cancellationToken) where T : class, IEntity
        {
            throw new NotImplementedException();
        }

        public Task AddRangeAsync<T>(List<T> entities, CancellationToken cancellationToken) where T : class, IEntity
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync<T>(T entity, CancellationToken cancellationToken) where T : class, IEntity
        {
            throw new NotImplementedException();
        }

        public Task RemoveRangeAsync<T>(List<T> entities, CancellationToken cancellationToken) where T : class, IEntity
        {
            throw new NotImplementedException();
        }
    }
}