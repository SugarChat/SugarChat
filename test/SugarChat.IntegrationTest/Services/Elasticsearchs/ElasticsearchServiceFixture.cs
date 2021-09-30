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
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using TestStack.BDDfy;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Requests.Groups;

namespace SugarChat.IntegrationTest.Services.Elasticsearchs
{
    public class ElasticsearchServiceFixture : TestBase
    {
        private async Task ShouldSyncMessage()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                for (int i = 0; i < 5; i++)
                {
                    await repository.AddAsync(new Core.Domain.Message
                    {
                        Id = Guid.NewGuid().ToString(),
                        GroupId = Guid.NewGuid().ToString(),
                        Content = "12345Content0" + i,
                        Type = i,
                        SentBy = Guid.NewGuid().ToString(),
                        SentTime = DateTime.Now.AddHours(i),
                        CreatedBy = Guid.NewGuid().ToString(),
                        CustomProperties = new Dictionary<string, string> { { "test1", "a" + i }, { "test2", "b" + (i + 3) } }
                    });
                }
                for (int i = 0; i < 5; i++)
                {
                    await repository.AddAsync(new Core.Domain.Message
                    {
                        Id = Guid.NewGuid().ToString(),
                        GroupId = Guid.NewGuid().ToString(),
                        Content = "45678Content1" + i,
                        Type = i,
                        SentBy = Guid.NewGuid().ToString(),
                        SentTime = DateTime.Now.AddHours(i),
                        CreatedBy = Guid.NewGuid().ToString(),
                        CustomProperties = new Dictionary<string, string> { { "test1", "a" + i }, { "test2", "b" + (i + 5) } }
                    });
                }
                await mediator.SendAsync(new SyncMessageToElasticsearchCommand());
            });
            await Task.Delay(1000);
            await Run<IElasticsearchDataProvider, IConfiguration>(async (elasticsearchDataProvider, configuration) =>
            {
                SearchRequest searchRequest = new SearchRequest<ElasticsearchMessage>(configuration["Elasticsearch:MessageIndex"]);
                var response = await elasticsearchDataProvider.SearchAsync<ElasticsearchMessage>(searchRequest, default(CancellationToken));
                response.total.ShouldBe(10);
            });
        }

        private async Task ShouldGetConversationByKeyword()
        {
            await Run<IElasticsearchService>(async elasticsearchService =>
            {
                {
                    GetConversationByKeywordRequest request = new GetConversationByKeywordRequest
                    {
                        IsExactSearch = true,
                        SearchParms = new Dictionary<string, string> { { "Content", "Content00" }, { "test1", "A1" } },
                        PageSettings = new Message.Paging.PageSettings { PageNum = 1, PageSize = 20 },
                    };
                    var response = await elasticsearchService.GetConversationByKeyword(request, default(CancellationToken));
                    response.total.ShouldBe(3);
                }
                {
                    GetConversationByKeywordRequest request = new GetConversationByKeywordRequest
                    {
                        IsExactSearch = true,
                        SearchParms = new Dictionary<string, string> { { "test2", "B3" } },
                        PageSettings = new Message.Paging.PageSettings { PageNum = 1, PageSize = 20 }
                    };
                    var response = await elasticsearchService.GetConversationByKeyword(request, default(CancellationToken));
                    response.total.ShouldBe(1);
                }
                {
                    GetConversationByKeywordRequest request = new GetConversationByKeywordRequest
                    {
                        IsExactSearch = false,
                        SearchParms = new Dictionary<string, string> { { "test2", "5" } },
                        PageSettings = new Message.Paging.PageSettings { PageNum = 1, PageSize = 20 }
                    };
                    var response = await elasticsearchService.GetConversationByKeyword(request, default(CancellationToken));
                    response.total.ShouldBe(2);
                }
            });
        }

        [Fact]
        public void RunMessageTest()
        {
            this.Given(x => x.ShouldSyncMessage())
                .And(x => x.ShouldGetConversationByKeyword())
                .BDDfy();
        }

        private async Task ShouldSyncGroup()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                List<Group> groups = new List<Group>();
                for (int i = 0; i < 5; i++)
                {
                    groups.Add(new Group
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "NameA nameB " + i,
                        Description = "description" + (i + 10) + "description",
                        CustomProperties = new Dictionary<string, string> { { "test1", "a" + i }, { "test2", "b" + (i + 3) } }
                    });
                }
                for (int i = 0; i < 5; i++)
                {
                    groups.Add(new Group
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "NameA nameB " + i,
                        Description = "description" + (i + 20) + "description",
                        CustomProperties = new Dictionary<string, string> { { "test1", "a" + i }, { "test2", "b" + (i + 5) } }
                    });
                }
                await repository.AddRangeAsync(groups);
                await mediator.SendAsync(new SyncGroupToElasticsearchCommand());
            });
            await Task.Delay(1000);
            await Run<IElasticsearchDataProvider, IConfiguration>(async (elasticsearchDataProvider, configuration) =>
            {
                SearchRequest searchRequest = new SearchRequest<ElasticsearchGroup>(configuration["Elasticsearch:GroupIndex"]);
                var response = await elasticsearchDataProvider.SearchAsync<ElasticsearchGroup>(searchRequest, default(CancellationToken));
                response.total.ShouldBe(10);
            });
        }

        private async Task ShouldGetGroupByCustomProperties()
        {
            await Run<IElasticsearchService>(async elasticsearchService =>
            {
                {
                    GetGroupByCustomPropertiesRequest request = new GetGroupByCustomPropertiesRequest
                    {
                        CustomProperties = new Dictionary<string, string> { { "test1", "A0" } }
                    };
                    var response = await elasticsearchService.GetGroupByCustomProperties(request, default(CancellationToken));
                    response.total.ShouldBe(2);
                }
                {
                    GetGroupByCustomPropertiesRequest request = new GetGroupByCustomPropertiesRequest
                    {
                        CustomProperties = new Dictionary<string, string> { { "test1", "A1" }, { "test2", "B4" } }
                    };
                    var response = await elasticsearchService.GetGroupByCustomProperties(request, default(CancellationToken));
                    response.total.ShouldBe(1);
                }
            });
        }

        [Fact]
        public void RunGroupTest()
        {
            this.Given(x => x.ShouldSyncGroup())
                .And(x => x.ShouldGetGroupByCustomProperties())
                .BDDfy();
        }
    }
}
