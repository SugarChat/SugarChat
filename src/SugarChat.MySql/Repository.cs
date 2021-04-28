﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Database.MySql
{
    public class Repository : IRepository
    {
        private readonly DbContext _dbContext;

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public Task SaveAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public IQueryable Set<T>() where T : class, IEntity
        {
            return _dbContext.Set<T>();
        }

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

        Task IRepository.AddAsync<T>(T eneity, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
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
