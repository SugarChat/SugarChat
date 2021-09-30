using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Elasticsearch;
using SugarChat.Message.Commands.Elasticsearchs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Elasticsearchs
{
    public class SyncGroupToElasticsearchCommandHandler : ICommandHandler<SyncGroupToElasticsearchCommand, SugarChatResponse>
    {
        private readonly IElasticsearchService _elasticsearchService;

        public SyncGroupToElasticsearchCommandHandler(IElasticsearchService elasticsearchService)
        {
            _elasticsearchService = elasticsearchService;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<SyncGroupToElasticsearchCommand> context, CancellationToken cancellationToken)
        {
            var syncGroupToElasticsearchEvent = await _elasticsearchService.SyncGroupAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(syncGroupToElasticsearchEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
