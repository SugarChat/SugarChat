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

namespace SugarChat.Core.Mediator.CommandHandlers.Elasticsearch
{
    public class SyncMessageToElasticsearchCommandHandler : ICommandHandler<SyncMessageToElasticsearchCommand, SugarChatResponse>
    {
        private readonly IElasticsearchService _elasticsearchService;

        public SyncMessageToElasticsearchCommandHandler(IElasticsearchService elasticsearchService)
        {
            _elasticsearchService = elasticsearchService;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<SyncMessageToElasticsearchCommand> context, CancellationToken cancellationToken)
        {
            var syncMessageToElasticsearchEvent = await _elasticsearchService.SyncMessageAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(syncMessageToElasticsearchEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
