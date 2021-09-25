using AutoMapper;
using Microsoft.Extensions.Configuration;
using Nest;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Commands.Elasticsearchs;
using SugarChat.Message.Dtos;
using SugarChat.Message.Events.Elasticsearchs;
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
            await _elasticsearchDataProvider.BatchCreateAsync(_configuration["Elasticsearch:MessageIndex"], elasticsearchMessages, cancellationToken);

            return _mapper.Map<SyncMessageToElasticsearchEvent>(command);
        }
    }
}
