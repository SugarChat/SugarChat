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
    public interface IElasticsearchService : IService
    {
        Task<SyncMessageToElasticsearchEvent> SyncMessageAsync(SyncMessageToElasticsearchCommand command, CancellationToken cancellationToken);
    }
}
