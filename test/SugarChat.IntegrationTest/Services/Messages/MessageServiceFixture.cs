using Mediator.Net;
using Shouldly;
using SugarChat.Core.Basic;
using SugarChat.Message.Requests.Messages;
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
                response.Data.ShouldBe(7);
            });
        }
    }
}
