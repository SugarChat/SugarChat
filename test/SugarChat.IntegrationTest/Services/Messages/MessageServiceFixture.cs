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
                        UserId = userId
                    };
                    var response = await mediator.RequestAsync<GetUnreadMessageCountRequest, SugarChatResponse<int>>(request);
                    response.Data.ShouldBe(5);
                }
                {
                    var request = new GetUnreadMessageCountRequest()
                    {
                        UserId = userId,
                        GroupIds = new string[] { groups[3].Id }
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
                    groupUser.LastReadTime.ToString().ShouldBe(lastMessage.SentTime.ToString());
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
                response.Data.Count().ShouldBe(8);
            });
        }

        [Fact]
        public async Task ShouldSetMessageReadByUserIdsBasedOnGroupId()
        {
            var groupId = groups.SingleOrDefault(o=>o.Id == groupId4)?.Id;
            var userIds = groupUsers.Where(o => o.GroupId == groupId).Select(o => o.UserId).ToList();
            userIds.Count().ShouldBeGreaterThan(1);
            
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                var lastMessage =
                    await repository.FirstOrDefaultAsync<Core.Domain.Message>(x => x.GroupId == groupId);
                var lastReadTime =
                    (await repository.ToListAsync<GroupUser>(
                        x => x.GroupId == groupId)).Select(o => o.LastReadTime).Distinct().ToList();
                lastReadTime.FirstOrDefault().ToString().ShouldNotBe(lastMessage.SentTime.ToString());
                
                var command = new SetMessageReadByUserIdsBasedOnGroupIdCommand()
                {
                    UserIds = userIds,
                    GroupId = groupId
                };
                await mediator.SendAsync<SetMessageReadByUserIdsBasedOnGroupIdCommand, SugarChatResponse>(command);

                lastReadTime =
                    (await repository.ToListAsync<GroupUser>(
                        x => x.GroupId == groupId)).Select(o => o.LastReadTime).Distinct().ToList();

                lastReadTime.Count().ShouldBe(1);
                lastReadTime.FirstOrDefault().ShouldNotBeNull();
                lastReadTime.FirstOrDefault().ToString().ShouldBe(lastMessage.SentTime.ToString());
            });
        }
    }
}
