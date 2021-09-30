using SugarChat.Core.Domain;
using SugarChat.Message.Commands.Elasticsearchs;
using SugarChat.Message.Dtos;
using SugarChat.Message.Events.Elasticsearchs;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Requests.Groups;
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

        Task<(IEnumerable<ElasticsearchMessage> list, int total)> GetConversationByKeyword(GetConversationByKeywordRequest request, CancellationToken cancellationToken);

        Task<SyncGroupToElasticsearchEvent> SyncGroupAsync(SyncGroupToElasticsearchCommand command, CancellationToken cancellationToken);

        Task<(IEnumerable<ElasticsearchGroup> list, int total)> GetGroupByCustomProperties(GetGroupByCustomPropertiesRequest request, CancellationToken cancellationToken);
    }
}
