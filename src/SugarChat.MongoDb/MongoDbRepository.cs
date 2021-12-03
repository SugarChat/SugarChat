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
using SugarChat.Core.Services;
using SugarChat.Data.MongoDb.Settings;
using SugarChat.Message.Paging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

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

        public async Task<List<T>> ToListAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            return await FilteredQuery(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<PagedResult<T>> ToPagedListAsync<T>(PageSettings pageSettings,
            Expression<Func<T, bool>> predicate = null,
            CancellationToken cancellationToken = default) where T : class, IEntity
        {
            var query = FilteredQuery(predicate);
            var result = await query.Paging(pageSettings).ToListAsync(cancellationToken).ConfigureAwait(false);
            var total = await query.CountAsync(cancellationToken).ConfigureAwait(false);
            return new PagedResult<T> { Result = result, Total = total };
        }

        public Task<PagedResult<T>> ToPagedListAsync<T>(PageSettings pageSettings, IQueryable<T> query,
            CancellationToken cancellationToken = default) where T : class, IEntity
        {
            List<T> result = new List<T>();
            if (pageSettings is not null)
            {
                if (pageSettings is not null)
                {
                    result = query.Paging(pageSettings).ToList();
                }
                else
                {
                    result = query.ToList();
                }
            }
            var total = query?.Count() ?? 0;
            return Task.FromResult(new PagedResult<T> { Result = result, Total = total });
        }

        public async Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate = null,
            CancellationToken cancellationToken = default) where T : class, IEntity
        {
            return await FilteredQuery(predicate).CountAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<T> SingleOrDefaultAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            return await FilteredQuery(predicate).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<T> SingleAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            return await FilteredQuery(predicate).SingleAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            return await FilteredQuery(predicate).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            return await FilteredQuery(predicate)
                    .AnyAsync(cancellationToken)
                    .ConfigureAwait(false);
        }

        public IMongoQueryable<T> Query<T>() where T : class, IEntity
        {
            return GetCollection<T>()
                .AsQueryable();
        }

        public async Task<int> AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            if (entity != null)
            {
                entity.CreatedDate = DateTimeOffset.Now;
                await GetCollection<T>().InsertOneAsync(entity, null, cancellationToken).ConfigureAwait(false);
                return 1;
            }
            return default;
        }

        public async Task<int> AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            if (entities?.Any() == true)
            {
                foreach (var entity in entities)
                {
                    entity.CreatedDate = DateTimeOffset.Now;
                }
                await GetCollection<T>().InsertManyAsync(entities, null, cancellationToken).ConfigureAwait(false);
                return entities.Count();
            }
            return default;
        }

        public async Task<int> RemoveAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            if (entity != null)
            {
                FilterDefinition<T> filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
                var deleteResult = await GetCollection<T>().DeleteOneAsync(filter, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (deleteResult.IsAcknowledged)
                {
                    return (int)deleteResult.DeletedCount;
                }
            }
            return default;
        }

        public async Task<int> RemoveRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            if (entities?.Any() == true)
            {
                FilterDefinition<T> filter = Builders<T>.Filter.In(e => e.Id, entities.Select(e => e.Id));
                var deleteCount = await GetCollection<T>().DeleteManyAsync(filter, null, cancellationToken).ConfigureAwait(false);
                if (deleteCount.IsAcknowledged)
                {
                    return (int)deleteCount.DeletedCount;
                }
            }
            return default;
        }

        public async Task<int> UpdateAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            if (entity != null)
            {
                var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
                entity.LastModifyDate = DateTimeOffset.Now;
                var replaceResult = await GetCollection<T>().ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (replaceResult.IsAcknowledged)
                {
                    return (int)replaceResult.ModifiedCount;
                }
            }
            return default;
        }

        public async Task<int> UpdateRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class, IEntity
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
                var writeResult = await GetCollection<T>().BulkWriteAsync(updates, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (writeResult.IsAcknowledged)
                {
                    return (int)writeResult.ModifiedCount;
                }
            }
            return default;
        }

        public async Task<IAsyncCursor<BsonDocument>> GetAggregate<T>(IEnumerable<string> stages, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            IList<IPipelineStageDefinition> pipelineStages = new List<IPipelineStageDefinition>();
            foreach (var stage in stages)
            {
                PipelineStageDefinition<BsonDocument, BsonDocument> pipelineStage = new JsonPipelineStageDefinition<BsonDocument, BsonDocument>(stage);
                pipelineStages.Add(pipelineStage);
            }

            PipelineDefinition<BsonDocument, BsonDocument> pipeline = new PipelineStagePipelineDefinition<BsonDocument, BsonDocument>(pipelineStages);
            return await _database.GetCollection<BsonDocument>(typeof(T).Name).AggregateAsync(pipeline, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<TDestination>> GetList<TSource, TDestination>(IEnumerable<string> stages, CancellationToken cancellationToken = default) where TSource : class, IEntity where TDestination : class
        {
            var bsonDocuments = await (await GetAggregate<TSource>(stages, cancellationToken).ConfigureAwait(false)).ToListAsync(cancellationToken).ConfigureAwait(false);
            var list = new List<TDestination>();
            foreach (var bsonDocument in bsonDocuments)
            {
                var messageCountGroupByGroupId = BsonSerializer.Deserialize<TDestination>(bsonDocument);
                list.Add(messageCountGroupByGroupId);
            }
            return list;
        }
    }
}