using AutoMapper;
using Microsoft.Extensions.Configuration;
using Nest;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Commands.Elasticsearchs;
using SugarChat.Message.Dtos;
using SugarChat.Message.Events.Elasticsearchs;
using SugarChat.Message.Requests.Conversations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Elasticsearch
{
    public class ElasticsearchService : IElasticsearchService
    {
        private readonly IConfiguration _configuration;
        private IElasticClient _client;
        private readonly IMapper _mapper;
        private readonly IRepository _repository;
        private readonly IElasticsearchDataProvider _elasticsearchDataProvider;

        public ElasticsearchService(IMapper mapper, IElasticClient client, IConfiguration configuration, IRepository repository, IElasticsearchDataProvider elasticsearchDataProvider)
        {
            _mapper = mapper;
            _configuration = configuration;
            _client = client;
            _repository = repository;
            _elasticsearchDataProvider = elasticsearchDataProvider;
        }

        public async Task<SyncMessageToElasticsearchEvent> SyncMessageAsync(SyncMessageToElasticsearchCommand command, CancellationToken cancellationToken)
        {
            var elasticsearchMessages = _repository.Query<Domain.Message>().Select(x => new ElasticsearchMessage
            {
                Id = x.Id,
                GroupId = x.GroupId,
                Content = x.Content,
                SentBy = x.SentBy,
                SentTime = x.SentTime,
                CustomProperties = x.CustomProperties
            });
            if (!elasticsearchMessages.Any())
            {
                return _mapper.Map<SyncMessageToElasticsearchEvent>(command);
            }

            await _elasticsearchDataProvider.CreateMessageIndexAsync(_configuration["Elasticsearch:MessageIndex"], elasticsearchMessages.FirstOrDefault(), cancellationToken).ConfigureAwait(false);

            await _elasticsearchDataProvider.BatchCreateAsync(_configuration["Elasticsearch:MessageIndex"], elasticsearchMessages, cancellationToken).ConfigureAwait(false);

            return _mapper.Map<SyncMessageToElasticsearchEvent>(command);
        }

        public async Task<(IEnumerable<ElasticsearchMessage> list, int total)> GetConversationByKeyword(GetConversationByKeywordRequest request, CancellationToken cancellationToken)
        {
            var queries = new List<Func<QueryContainerDescriptor<ElasticsearchMessage>, QueryContainer>> { };

            var shouldQueries = new List<Func<QueryContainerDescriptor<ElasticsearchMessage>, QueryContainer>> { };
            foreach (var searchParm in request.SearchParms)
            {
                if (searchParm.Key.Equals(Message.Constant.Content))
                {
                    var contentKeyword = request.SearchParms.GetValueOrDefault(Message.Constant.Content);
                    if (!string.IsNullOrEmpty(contentKeyword))
                    {
                        var keyword = $"*{contentKeyword}*";
                        shouldQueries.Add(q => q.Wildcard(w => w.Field(f => f.Content).Value(keyword.ToLower())));
                    }
                }
                else
                {
                    if (request.IsExactSearch)
                    {
                        shouldQueries.Add(q => q.Term(t => t.Field("custom_properties." + searchParm.Key).Value(searchParm.Value.ToLower())));
                    }
                    else
                    {
                        var keyword = $"*{searchParm.Value}*";
                        shouldQueries.Add(q => q.Wildcard(w => w.Field("custom_properties." + searchParm.Key).Value(keyword.ToLower())));
                    }
                }

            }
            queries.Add(q => q.Bool(b => b.Should(shouldQueries)));

            Func<SearchDescriptor<ElasticsearchMessage>, ISearchRequest> searchRequest = s => s.Query(q => q.Bool(b => b.Filter(queries)))
                .Index(_configuration["Elasticsearch:MessageIndex"])
                .From((request.PageSettings.PageNum - 1) * request.PageSettings.PageSize)
                .Size(request.PageSettings.PageSize)
                .Sort(s => s.Descending(a => a.SentTime))
                .Source(s => s.Includes(i => i.Field(f => f.GroupId)));

            return await _elasticsearchDataProvider.SearchAsync(searchRequest, cancellationToken).ConfigureAwait(false);
        }
    }
}
