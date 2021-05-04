using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using MongoDB.Driver;
using SugarChat.Core.Settings;

namespace SugarChat.Data.MongoDb
{
    public class MongoDbRepository : IRepository
    {
        readonly IMongoDatabase _database;
        public MongoDbRepository(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public Task<List<T>> ToListAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class, IEntity
        {
            var list = GetCollection<T>()
                 .AsQueryable()
                 .Where(WhereAdapter(predicate))
                 .ToList();

            return Task.FromResult(list);
        }

        public Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class, IEntity
        {
            var result = GetCollection<T>()
                .AsQueryable()
                .Where(WhereAdapter(predicate))
                .Count();

            return Task.FromResult(result);
        }

        public Task<T> SingleOrDefaultAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class, IEntity
        {
            var result = GetCollection<T>()
              .AsQueryable()
              .Where(WhereAdapter(predicate))
              .SingleOrDefault();

            return Task.FromResult(result);
        }

        public Task<T> SingleAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class, IEntity
        {
            var result = GetCollection<T>()
               .AsQueryable()
               .Where(WhereAdapter(predicate))
               .Single();

            return Task.FromResult(result);
        }

        public Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class, IEntity
        {
            var result = GetCollection<T>()
                .AsQueryable()
                .Where(WhereAdapter(predicate))
                .FirstOrDefault();

            return Task.FromResult(result);
        }

        public Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class, IEntity
        {
            var result = GetCollection<T>()
                      .AsQueryable()
                      .Where(WhereAdapter(predicate))
                      .Any();

            return Task.FromResult(result);
        }

        public IQueryable<T> Query<T>(Expression<Func<T, bool>> predicate = null) where T : class, IEntity
        {
            return GetCollection<T>()
                   .AsQueryable();
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task AddAsync<T>(T entity, CancellationToken cancellationToken) where T : class, IEntity
        {
            return GetCollection<T>().InsertOneAsync(entity, null, cancellationToken);
        }

        public Task AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken) where T : class, IEntity
        {
            return GetCollection<T>().InsertManyAsync(entities, null, cancellationToken);
        }

        public Task RemoveAsync<T>(T entity, CancellationToken cancellationToken) where T : class, IEntity
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
            return GetCollection<T>().DeleteOneAsync(filter, null, cancellationToken);
        }

        public Task RemoveRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken) where T : class, IEntity
        {
            FilterDefinition<T> filter = Builders<T>.Filter.In(e => e.Id, entities.Select(e => e.Id));
            return GetCollection<T>().DeleteManyAsync(filter, null, cancellationToken);
        }

        private Expression<Func<T, bool>> WhereAdapter<T>(Expression<Func<T, bool>> expression)
        {
            return expression == null ? e => true : expression;
        }

        private IMongoCollection<T> GetCollection<T>()
        {
            return _database.GetCollection<T>(nameof(T));
        }
    }
}
