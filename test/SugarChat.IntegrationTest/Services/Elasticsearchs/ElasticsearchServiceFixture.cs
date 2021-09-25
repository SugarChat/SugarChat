using Mediator.Net;
using Microsoft.Extensions.Configuration;
using Nest;
using Newtonsoft.Json;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Services.Elasticsearch;
using SugarChat.Message.Commands.Elasticsearchs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.IntegrationTest.Services.Elasticsearchs
{
    public class ElasticsearchServiceFixture : TestBase
    {
        [Fact]
        public async Task ShouldSyncMessage()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                for (int i = 0; i < 10; i++)
                {
                    object payload = new
                    {
                        uuid = Guid.NewGuid(),
                        url = "testUrl" + i,
                        size = 100,
                        second = 50
                    };
                    await repository.AddAsync(new Core.Domain.Message
                    {
                        Id = Guid.NewGuid().ToString(),
                        GroupId = Guid.NewGuid().ToString(),
                        Content = "test" + i,
                        Type = i,
                        SentBy = Guid.NewGuid().ToString(),
                        SentTime = DateTime.Now.AddHours(i),
                        Payload = JsonConvert.SerializeObject(payload),
                        CreatedBy = Guid.NewGuid().ToString(),
                        CustomProperties = new Dictionary<string, string> { { "Number", "1" } }
                    });
                }
                await mediator.SendAsync(new SyncMessageToElasticsearchCommand());
            });
            await Task.Delay(1000);
            await Run<IElasticsearchDataProvider, IConfiguration>(async (elasticsearchDataProvider, configuration) =>
            {
                SearchRequest searchRequest = new SearchRequest<ElasticsearchMessage>(configuration["Elasticsearch:MessageIndex"]);
                var response = await elasticsearchDataProvider.SearchAsync<ElasticsearchMessage>(searchRequest);
                response.total.ShouldBe(10);
            });
        }
    }
}
