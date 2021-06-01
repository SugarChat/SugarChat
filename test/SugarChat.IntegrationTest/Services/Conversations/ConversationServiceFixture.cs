using Autofac;
using Mediator.Net;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Commands.Conversations;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.IntegrationTest.Services.Conversations
{
    public class ConversationServiceFixture : TestFixtureBase
    {
        [Fact]
        public async Task ShouldGetUserConversations()
        {
            await Run<IMediator>(async (mediator) =>
            {
                var reponse = await mediator.RequestAsync<GetConversationListRequest, GetConversationListResponse>
                (new GetConversationListRequest { UserId = userId });

                reponse.Result.Count().ShouldBe(2);
            });
        }

        [Fact]
        public async Task ShouldSetConversationMessagesRead()
        {
            await Run<IMediator>(async (mediator) =>
            {
                await mediator.SendAsync(new SetMessageAsReadCommand
                {
                    ConversationId = conversationId,
                    UserId = userId

                }, default(CancellationToken));

                var request = new GetConversationListRequest()
                {
                    UserId = userId
                };
                var response = await mediator.RequestAsync<GetConversationListRequest, GetConversationListResponse>(request);
                response.Result.Where(x => x.ConversationID == conversationId)
                .FirstOrDefault().UnreadCount.ShouldBe(0);
            });
        }

        [Fact]
        public async Task ShouldGetConversationProfile()
        {
            await Run<IMediator>(async (mediator) =>
            {
                var request = new GetConversationProfileRequest()
                {
                    ConversationId = conversationId,
                    UserId = userId
                };
                var response = await mediator.RequestAsync<GetConversationProfileRequest, GetConversationProfileResponse>(request);
                response.Result.Name.ShouldBe("TestGroup3");
            });
        }

        [Fact]
        public async Task ShouldGetMessageList()
        {
            await Run<IMediator>(async (mediator) =>
            {
                var request = new GetMessageListRequest()
                {
                    ConversationId = conversationId,
                    UserId = userId,
                    NextReqMessageId = "",
                    Count = 5
                };
                var response = await mediator.RequestAsync<GetMessageListRequest, GetMessageListResponse>(request);
                response.Result.Count().ShouldBe(3);
                response.Result.First().Content.ShouldBe("[图片]");
            });
        }


        [Fact]
        public async Task ShouldDeleteConversation()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await mediator.SendAsync(new DeleteConversationCommand
                {
                    ConversationId = conversationId,
                    UserId = userId

                }, default(CancellationToken));

                var groupUser = await repository.SingleOrDefaultAsync<GroupUser>(x => x.GroupId == conversationId && x.UserId == userId);
                groupUser.ShouldBeNull();
            });
        }

    }
}
