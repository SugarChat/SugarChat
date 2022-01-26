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
                await mediator.SendAsync<SetGroupMemberCustomFieldCommand, SugarChatResponse>(new SetGroupMemberCustomFieldCommand
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
        public async Task ShouldGetMembersOfGroup()
        {
            await Run<IMediator>(async (mediator) =>
            {
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

        [Fact]
        public async Task ShouldRemoveAllGroupMember()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                string[] userIds = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
                foreach (var userId in userIds)
                {
                    await repository.AddAsync(new User
                    {
                        Id = userId
                    });
                }
                var groupId = Guid.NewGuid().ToString();
                {
                    AddGroupCommand command = new AddGroupCommand
                    {
                        UserId = userIds[0],
                        Id = groupId,
                        CustomProperties = new Dictionary<string, string> { { "MerchId", "1" }, { "OrderId", "2" } },
                        CreatedBy = Guid.NewGuid().ToString()
                    };
                    await mediator.SendAsync<AddGroupCommand, SugarChatResponse>(command);
                    (await repository.CountAsync<Group>(x => x.Id == groupId)).ShouldBe(1);
                }
                {
                    AddGroupMemberCommand command = new AddGroupMemberCommand
                    {
                        GroupId = groupId,
                        AdminId = userIds[0],
                        GroupUserIds = userIds.Skip(1),
                        CreatedBy = userIds[0]
                    };
                    await mediator.SendAsync<AddGroupMemberCommand, SugarChatResponse>(command);
                    (await repository.CountAsync<GroupUser>(x => x.GroupId == groupId)).ShouldBe(3);
                }
                {
                    RemoveAllGroupMemberCommand command = new RemoveAllGroupMemberCommand
                    {
                        UserId = userIds[0],
                        GroupId = groupId
                    };
                    await mediator.SendAsync<RemoveAllGroupMemberCommand, SugarChatResponse>(command);
                    (await repository.CountAsync<GroupUser>(x => x.GroupId == groupId)).ShouldBe(0);
                }
            });
        }
    }
}
