using Mediator.Net;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Commands.GroupUsers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using System.Linq;

namespace SugarChat.IntegrationTest.Services.GroupUsers
{
    public class GroupUserServiceFixture : TestFixtureBase
    {
        [Fact]
        public async Task ShouldSetGroupMemberCustomField()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await mediator.SendAsync(new SetGroupMemberCustomFieldCommand
                {
                    GroupId = conversationId,
                    UserId = userId,
                    CustomProperties = new Dictionary<string, string>
                    {
                        { "Signature", "The closer you get to the essence, the less confused you will be" }
                    }

                }, default(CancellationToken));

                var groupUser = await repository.SingleOrDefaultAsync<GroupUser>(x => x.GroupId == conversationId && x.UserId == userId);
                groupUser.CustomProperties["Signature"].ShouldBe("The closer you get to the essence, the less confused you will be");
            });
        }

        [Fact]
        public async Task ShouldGetGroupMembers()
        {
            await Run<IMediator>(async (mediator) =>
            {
                var reponse = await mediator.RequestAsync<GetMembersOfGroupRequest, GetMembersOfGroupResponse>(new GetMembersOfGroupRequest { UserId = userId, GroupId = conversationId });
                reponse.Result.Count().ShouldBe(2);
            });
        }

    }
}
