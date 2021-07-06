using Mediator.Net;
using Shouldly;
using SugarChat.Core.Basic;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Requests.Messages;
using SugarChat.Shared.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.IntegrationTest.Services.Messages
{
    public class MessageServiceFixture : TestFixtureBase
    {
        [Fact]
        public async Task ShouldGetUnreadMessageCount()
        {
            await Run<IMediator>(async (mediator) =>
            {
                var request = new GetUnreadMessageCountRequest()
                {
                    UserId = userId
                };
                var response = await mediator.RequestAsync<GetUnreadMessageCountRequest, SugarChatResponse<int>>(request);
                response.Data.ShouldBe(8);
            });
        }

        [Fact]
        public async Task ShouldSetMessageReadByUserBasedOnGroupId()
        {
            await Run<IMediator>(async (mediator) =>
            {
                var command = new SetMessageReadByUserBasedOnGroupIdCommand()
                {
                    UserId = userId,
                    GroupId = groups[0].Id
                };
                var response = await mediator.SendAsync<SetMessageReadByUserBasedOnGroupIdCommand, SugarChatResponse>(command);
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
                response.Data.Count().ShouldBe(7);
            });
        }
    }
}
