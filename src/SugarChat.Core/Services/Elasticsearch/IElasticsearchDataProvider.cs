using Nest;
using SugarChat.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Elasticsearch
{
    public interface IElasticsearchDataProvider : IDataProvider
    {
        Task BatchCreateAsync<T>(string indexName, IEnumerable<T> entities, CancellationToken cancellationToken) where T : class;

        Task<(IEnumerable<T> list, int total)> SearchAsync<T>(SearchRequest searchRequest, CancellationToken cancellationToken) where T : class;

        Task<(IEnumerable<T> list, int total)> SearchAsync<T>(Func<SearchDescriptor<T>, ISearchRequest> searchRequest, CancellationToken cancellationToken) where T : class;

        Task CreateMessageIndexAsync(string indexName, ElasticsearchMessage message, CancellationToken cancellationToken);
    }
}
