using Autofac;
using Mediator.Net;
using Shouldly;
using SugarChat.Core.Basic;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Commands.Conversations;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
using SugarChat.Shared.Dtos.Conversations;
using System.Collections.Generic;
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
                var reponse = await mediator.RequestAsync<GetConversationListRequest, SugarChatResponse<IEnumerable<ConversationDto>>>
                (new GetConversationListRequest { UserId = userId });

                reponse.Data.Count().ShouldBe(2);
            });
        }

        [Fact]
        public async Task ShouldSetConversationMessagesRead()
        {
            await Run<IMediator>(async (mediator) =>
            {
                await mediator.SendAsync<SetMessageAsReadCommand, SugarChatResponse>(new SetMessageAsReadCommand
                {
                    ConversationId = conversationId,
                    UserId = userId

                }, default(CancellationToken));

                var request = new GetConversationListRequest()
                {
                    UserId = userId
                };
                var response = await mediator.RequestAsync<GetConversationListRequest, SugarChatResponse<IEnumerable<ConversationDto>>>(request);
                response.Data.Where(x => x.ConversationID == conversationId)
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
                var response = await mediator.RequestAsync<GetConversationProfileRequest, SugarChatResponse<ConversationDto>>(request);
                response.Data.GroupProfile.Name.ShouldBe("TestGroup3");
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
                var response = await mediator.RequestAsync<GetMessageListRequest, SugarChatResponse<MessageListResult>>(request);
                response.Data.Messages.Count().ShouldBe(3);
                response.Data.Messages.First().Content.ShouldBe("[图片]");
            });
        }

        [Fact]
        public async Task ShouldDeleteConversation()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await mediator.SendAsync<DeleteConversationCommand, SugarChatResponse> (new DeleteConversationCommand
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
