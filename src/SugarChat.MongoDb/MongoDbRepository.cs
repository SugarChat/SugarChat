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
using MongoDB.Driver.Linq;
using SugarChat.Data.MongoDb.Settings;

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

        private static Expression<Func<T, bool>> WhereAdapter<T>(Expression<Func<T, bool>> expression)
        {
            return expression ?? (e => true);
        }

        private IMongoCollection<T> GetCollection<T>()
        {
            return _database.GetCollection<T>(typeof(T).Name);
        }

        private IMongoQueryable<T> FilteredQuery<T>(Expression<Func<T, bool>> predicate = null)
        {
            return GetCollection<T>()
                   .AsQueryable()
                   .Where(WhereAdapter(predicate));
        }

        public Task<List<T>> ToListAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            return FilteredQuery(predicate).ToListAsync(cancellationToken);
        }

        public Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            return FilteredQuery(predicate).CountAsync(cancellationToken);
        }

        public Task<T> SingleOrDefaultAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            return FilteredQuery(predicate).SingleOrDefaultAsync(cancellationToken);
        }

        public Task<T> SingleAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            return FilteredQuery(predicate).SingleAsync(cancellationToken);
        }

        public Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            return FilteredQuery(predicate).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            return FilteredQuery(predicate).AnyAsync(cancellationToken);
        }

        public IQueryable<T> Query<T>() where T : class, IEntity
        {
            return GetCollection<T>()
                   .AsQueryable();
        }

        public Task AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            if (entity == null)
            {
                return Task.CompletedTask;
            }
            entity.CreatedDate = DateTimeOffset.Now;
            return GetCollection<T>().InsertOneAsync(entity, null, cancellationToken);
        }

        public Task AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            if (entities?.Any() == true)
            {
                foreach (var entity in entities)
                {
                    entity.CreatedDate = DateTimeOffset.Now;
                }
                return GetCollection<T>().InsertManyAsync(entities, null, cancellationToken);
            }
            return Task.CompletedTask;
        }

        public Task RemoveAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            if (entity == null)
            {
                return Task.CompletedTask;
            }
            FilterDefinition<T> filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
            return GetCollection<T>().DeleteOneAsync(filter, null, cancellationToken);
        }

        public Task RemoveRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            if (entities?.Any() == true)
            {
                FilterDefinition<T> filter = Builders<T>.Filter.In(e => e.Id, entities.Select(e => e.Id));
                return GetCollection<T>().DeleteManyAsync(filter, null, cancellationToken);
            }
            return Task.CompletedTask;
        }

        public Task UpdateAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            if (entity == null)
            {
                return Task.CompletedTask;
            }
            var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
            entity.LastModifyDate = DateTimeOffset.Now;
            return GetCollection<T>().ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
        }

        public Task UpdateRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            if (entities?.Any() == true)
            {
                var updates = new List<WriteModel<T>>();
                foreach (var entity in entities)
                {
                    entity.LastModifyDate = DateTimeOffset.Now;
                    var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
                    updates.Add(new ReplaceOneModel<T>(filter, entity));
                }
                return GetCollection<T>().BulkWriteAsync(updates, cancellationToken: cancellationToken);
            }
            return Task.CompletedTask;
        }
    }
}
