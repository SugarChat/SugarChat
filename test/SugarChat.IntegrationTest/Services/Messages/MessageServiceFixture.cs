using Mediator.Net;
using Shouldly;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Requests.Messages;
using SugarChat.Message.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Domain;
using System;
using SugarChat.Message.Basic;

namespace SugarChat.IntegrationTest.Services.Messages
{
    public class MessageServiceFixture : TestFixtureBase
    {
        [Fact]
        public async Task ShouldGetUnreadMessageCount()
        {
            await Run<IMediator>(async (mediator) =>
            {
                {
                    var request = new GetUnreadMessageCountRequest()
                    {
                        UserId = userId,
                        GroupType = 10
                    };
                    var response = await mediator.RequestAsync<GetUnreadMessageCountRequest, SugarChatResponse<int>>(request);
                    response.Data.ShouldBe(6);
                }
                {
                    var request = new GetUnreadMessageCountRequest()
                    {
                        UserId = userId,
                        GroupType = 10,
                        SearchParams = new List<SearchParamDto> { new SearchParamDto
                        {
                            SearchParamDetails = new List<SearchParamDetail>
                            {
                                new SearchParamDetail
                                {
                                    Key = "A",
                                    Value = "3AB",
                                    ConditionCondition = Condition.Unequal
                                }
                            }
                        } }
                    };
                    var response = await mediator.RequestAsync<GetUnreadMessageCountRequest, SugarChatResponse<int>>(request);
                    response.Data.ShouldBe(3);
                }
                {
                    var request = new GetUnreadMessageCountRequest()
                    {
                        UserId = userId,
                        GroupType = 10,
                        SearchParams = new List<SearchParamDto> { new SearchParamDto
                        {
                            SearchParamDetails = new List<SearchParamDetail>
                            {
                                new SearchParamDetail
                                {
                                    Key = "A",
                                    Value = "3AB"
                                }
                            }
                        } }
                    };
                    var response = await mediator.RequestAsync<GetUnreadMessageCountRequest, SugarChatResponse<int>>(request);
                    response.Data.ShouldBe(3);
                }
                {
                    var request = new GetUnreadMessageCountRequest()
                    {
                        UserId = userId,
                        GroupType = 10,
                        SearchParams = new List<SearchParamDto> { new SearchParamDto
                        {
                            SearchParamDetails = new List<SearchParamDetail>
                            {
                                new SearchParamDetail
                                {
                                    Key = "A",
                                    Value = "3",
                                    ConditionCondition = Condition.Contain
                                }
                            }
                        } }
                    };
                    var response = await mediator.RequestAsync<GetUnreadMessageCountRequest, SugarChatResponse<int>>(request);
                    response.Data.ShouldBe(3);
                }
                {
                    var request = new GetUnreadMessageCountRequest()
                    {
                        UserId = userId,
                        GroupType = 10,
                        SearchParams = new List<SearchParamDto> { new SearchParamDto
                        {
                            SearchParamDetails = new List<SearchParamDetail>
                            {
                                new SearchParamDetail
                                {
                                    Key = "A",
                                    Value = "3AB",
                                    ConditionCondition = Condition.Unequal
                                }
                            }
                        }, new SearchParamDto
                        {
                            SearchParamDetails = new List<SearchParamDetail>
                            {
                                new SearchParamDetail
                                {
                                    Key = "A",
                                    Value = "3AB"
                                }
                            }
                        } }
                    };
                    var response = await mediator.RequestAsync<GetUnreadMessageCountRequest, SugarChatResponse<int>>(request);
                    response.Data.ShouldBe(0);
                }
                {
                    var request = new GetUnreadMessageCountRequest()
                    {
                        UserId = userId,
                        GroupType = 10,
                        SearchParams = new List<SearchParamDto> { new SearchParamDto
                        {
                            SearchParamDetails = new List<SearchParamDetail>
                            {
                                new SearchParamDetail
                                {
                                    Key = "A",
                                    Value = "3AB",
                                    ConditionCondition = Condition.Unequal
                                }
                            }
                        }, new SearchParamDto
                        {
                            SearchParamDetails = new List<SearchParamDetail>
                            {
                                new SearchParamDetail
                                {
                                    Key = "A",
                                    Value = "4AB"
                                }
                            }
                        } }
                    };
                    var response = await mediator.RequestAsync<GetUnreadMessageCountRequest, SugarChatResponse<int>>(request);
                    response.Data.ShouldBe(1);
                }
                {
                    var request = new GetUnreadMessageCountRequest()
                    {
                        UserId = userId2,
                        GroupType = 10
                    };
                    var response = await mediator.RequestAsync<GetUnreadMessageCountRequest, SugarChatResponse<int>>(request);
                    response.Data.ShouldBe(3);
                }
                {
                    var request = new GetUnreadMessageCountRequest()
                    {
                        UserId = userId,
                        GroupIds = new string[] { groups[3].Id },
                        GroupType = 10
                    };
                    var response = await mediator.RequestAsync<GetUnreadMessageCountRequest, SugarChatResponse<int>>(request);
                    response.Data.ShouldBe(3);
                }
            });
        }

        [Fact]
        public async Task ShouldSetMessageReadByUserBasedOnGroupId()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                for (int i = 0; i < 2; i++)
                {
                    var command = new SetMessageReadByUserBasedOnGroupIdCommand()
                    {
                        UserId = userId,
                        GroupId = groups[2].Id
                    };
                    var response = await mediator.SendAsync<SetMessageReadByUserBasedOnGroupIdCommand, SugarChatResponse>(command);
                    var groupUser = await repository.FirstOrDefaultAsync<GroupUser>(x => x.UserId == userId && x.GroupId == groups[2].Id);
                    var lastMessage = await repository.FirstOrDefaultAsync<Core.Domain.Message>(x => x.GroupId == groups[2].Id);
                    groupUser.UnreadCount.ShouldBe(0);
                }
            });
        }

        [Fact]
        public async Task ShouldGetMessagesByGroupIds()
        {
            await Run<IMediator>(async (mediator) =>
            {
                var request = new GetMessagesByGroupIdsRequest()
                {
                    UserId = userId,
                    GroupIds = groups.Select(x => x.Id).ToArray()
                };
                var response = await mediator.RequestAsync<GetMessagesByGroupIdsRequest, SugarChatResponse<IEnumerable<MessageDto>>>(request);
                response.Data.Count().ShouldBe(9);
            });
        }
    }
}
