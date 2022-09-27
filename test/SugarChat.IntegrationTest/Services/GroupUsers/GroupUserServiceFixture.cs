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
using System.Linq;
using SugarChat.Message.Dtos.GroupUsers;
using SugarChat.Message.Requests.GroupUsers;
using SugarChat.Message.Basic;
using System;
using SugarChat.Message.Commands.Groups;

namespace SugarChat.IntegrationTest.Services.GroupUsers
{
    public class GroupUserServiceFixture : TestFixtureBase
    {
        [Fact]
        public async Task ShouldSetGroupMemberCustomField()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await repository.AddAsync(new GroupUserCustomProperty { GroupUserId = groupUsers[7].Id, Key = "Key", Value = "Value" });
                {
                    var reponse = await mediator.RequestAsync<GetMembersOfGroupRequest, SugarChatResponse<IEnumerable<GroupUserDto>>>(new GetMembersOfGroupRequest { UserId = userId, GroupId = conversationId });
                    reponse.Data.First(x => x.UserId == userId).CustomPropertyList.Count().ShouldBe(1);
                }
                await mediator.SendAsync<SetGroupMemberCustomFieldCommand, SugarChatResponse>(new SetGroupMemberCustomFieldCommand
                {
                    GroupId = conversationId,
                    UserId = userId,
                    CustomProperties = new Dictionary<string, string>
                    {
                        { "Signature", "The closer you get to the essence, the less confused you will be" }
                    }

                }, default(CancellationToken));
                {
                    var reponse = await mediator.RequestAsync<GetMembersOfGroupRequest, SugarChatResponse<IEnumerable<GroupUserDto>>>(new GetMembersOfGroupRequest { UserId = userId, GroupId = conversationId });
                    reponse.Data.First(x => x.UserId == userId).CustomPropertyList.Count(x => x.Key == "Signature").ShouldBe(1);
                    var groupUser = await repository.SingleOrDefaultAsync<GroupUser>(x => x.GroupId == conversationId && x.UserId == userId);
                }
            });
        }

        [Fact]
        public async Task ShouldGetMembersOfGroup()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await repository.AddAsync(new GroupUserCustomProperty { GroupUserId = groupUsers[7].Id, Key = "Key", Value = "Value" });
                var reponse = await mediator.RequestAsync<GetMembersOfGroupRequest, SugarChatResponse<IEnumerable<GroupUserDto>>>(new GetMembersOfGroupRequest { UserId = userId, GroupId = conversationId });
                reponse.Data.Count().ShouldBe(2);
            });
        }

        [Fact]
        public async Task ShouldGetGroupMembers()
        {
            await Run<IMediator>(async (mediator) =>
            {
                var reponse = await mediator.RequestAsync<GetGroupMembersRequest, SugarChatResponse<IEnumerable<string>>>(new GetGroupMembersRequest { GroupId = conversationId });
                reponse.Data.Count().ShouldBe(2);
            });
        }

        [Fact]
        public async Task ShouldGetUserIdsByGroupIds()
        {
            await Run<IMediator>(async (mediator) =>
            {
                var reponse = await mediator.RequestAsync<GetUserIdsByGroupIdsRequest, SugarChatResponse<IEnumerable<string>>>(new GetUserIdsByGroupIdsRequest { GroupIds = new List<string> { conversationId } });
                reponse.Data.Count().ShouldBe(2);
            });
        }
    }
}
