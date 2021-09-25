using Nest;
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
        private IElasticClient _client;

        public ElasticsearchDataProvider(IElasticClient client)
        {
            _client = client;
        }

        private ConnectionSettings GetIndex(string indexName)
        {
            return new ConnectionSettings().DefaultIndex(indexName);
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

        public async Task<(IEnumerable<T> list, int total)> SearchAsync<T>(SearchRequest searchRequest) where T : class
        {
            var response = await _client.SearchAsync<T>(searchRequest);
            return (response.Documents, (int)response.Total);
        }
    }
}
