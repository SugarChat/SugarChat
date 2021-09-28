using Nest;
using Serilog;
using SugarChat.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Elasticsearch
{
    public class ElasticsearchDataProvider : IElasticsearchDataProvider
    {
        public const string KeywordName = "keyword";
        public const string WildcardName = "wildcard";

        private IElasticClient _client;

        public ElasticsearchDataProvider(IElasticClient client)
        {
            _client = client;
        }

        private ConnectionSettings GetIndex(string indexName)
        {
            return new ConnectionSettings().DefaultIndex(indexName);
        }

        private static void CheckResponse(CreateIndexResponse response)
        {
            if (response.Acknowledged)
                Log.Information("Init {@index} Indices", response.Index);
            else
                Log.Error(response.OriginalException, "Init {@index} Indices error");
        }

        private async Task<bool> IndexExists(string indexName)
        {
            var existResponse = await _client.Indices.ExistsAsync(indexName);
            return existResponse.Exists;
        }

        private async Task CreateIndexAsync<T>(string indexName) where T : class
        {
            if (await IndexExists(indexName)) return;

            var response = await _client.Indices.CreateAsync(indexName, i => i.Map<T>(m => m.AutoMap()));

            if (response.Acknowledged)
                Log.Information("Init {@index} Indices", response.Index);
            else
                Log.Error(response.OriginalException, "Init {@index} Indices error");
        }

        public async Task CreateMessageIndexAsync(string indexName, ElasticsearchMessage message, CancellationToken cancellationToken)
        {
            if (await IndexExists(indexName)) return;

            Func<PropertiesDescriptor<ElasticsearchMessage>, IPromise<IProperties>> func = p => p
                .Keyword(k => k.Name(n => n.Id))
                .Keyword(k => k.Name(n => n.GroupId))
                .Text(t => t.Name(n => n.Content)
                    .Fields(f => f.Wildcard(k => k.Name(WildcardName))))
                .Keyword(k => k.Name(n => n.SentBy))
                .Date(d => d.Name(n => n.SentTime));

            if (message.CustomProperties is not null && message.CustomProperties.Any())
            {
                foreach (var customProperty in message.CustomProperties)
                {
                    func += p => p.Text(k => k.Name("custom_properties." + customProperty.Key).Fields(f => f.Wildcard(k => k.Name(WildcardName))));
                }
            }

            var response = await _client.Indices.CreateAsync(indexName, i => i.Map<ElasticsearchMessage>(m => m
                .Properties(func)
                .AutoMap()));

            CheckResponse(response);
        }

        public async Task BatchCreateAsync<T>(string indexName, IEnumerable<T> entities, CancellationToken cancellationToken) where T : class
        {
            if (entities == null || !entities.Any())
            {
                return;
            }
            var bulkOperation = new BulkOperationsCollection<IBulkOperation>();
            var bulkRequest = new BulkRequest(indexName) { Operations = bulkOperation };
            foreach (var entity in entities)
            {
                bulkRequest.Operations.Add(new BulkCreateOperation<T>(entity));
            }
            var response = await _client.BulkAsync(bulkRequest, cancellationToken);
            if (!response.IsValid)
            {
                throw new Exception($"BatchCreate {indexName}:{entities.FirstOrDefault()} Error");
            }
        }

        public async Task<(IEnumerable<T> list, int total)> SearchAsync<T>(SearchRequest searchRequest, CancellationToken cancellationToken) where T : class
        {
            var response = await _client.SearchAsync<T>(searchRequest);
            return (response.Documents, (int)response.Total);
        }

        public async Task<(IEnumerable<T> list, int total)> SearchAsync<T>(Func<SearchDescriptor<T>, ISearchRequest> searchRequest, CancellationToken cancellationToken) where T : class
        {
            var response = await _client.SearchAsync(searchRequest);
            return (response.Documents, (int)response.Total);
        }
    }
}
