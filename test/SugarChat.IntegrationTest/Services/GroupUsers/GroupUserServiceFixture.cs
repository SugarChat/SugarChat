using Mediator.Net;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Commands.GroupUsers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using SugarChat.Message.Requests;
using System.Linq;
using SugarChat.Message.Dtos.GroupUsers;
using SugarChat.Message.Requests.GroupUsers;
using SugarChat.Message.Basic;
using System;

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

                }, default);
                {
                    var reponse = await mediator.RequestAsync<GetMembersOfGroupRequest, SugarChatResponse<IEnumerable<GroupUserDto>>>(new GetMembersOfGroupRequest { UserId = userId, GroupId = conversationId });
                    reponse.Data.First(x => x.UserId == userId).CustomProperties.Count().ShouldBe(3);
                    reponse.Data.First(x => x.UserId == userId).CustomProperties.Count(x => x.Key == "Signature").ShouldBe(1);
                    reponse.Data.First(x => x.UserId == userId).CustomProperties.Count(x => x.Key == "A" && x.Value == "2AB").ShouldBe(1);
                    var groupUser = await repository.SingleOrDefaultAsync<GroupUser>(x => x.GroupId == conversationId && x.UserId == userId);
                }
            });
        }

        [Fact]
        public async Task ShouldBatchSetGroupMemberCustomFieldCommand()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await mediator.SendAsync<BatchSetGroupMemberCustomFieldCommand, SugarChatResponse>(new BatchSetGroupMemberCustomFieldCommand
                {
                    SetGroupMemberCustomFieldCommands = new List<SetGroupMemberCustomFieldCommand>
                    {
                        new SetGroupMemberCustomFieldCommand
                        {
                            GroupId = conversationId,
                            UserId = userId,
                            CustomProperties = new Dictionary<string, string>
                            {
                                { "Signature", "The closer you get to the essence, the less confused you will be" }
                            }
                        },
                        new SetGroupMemberCustomFieldCommand
                        {
                            GroupId = groupId1,
                            UserId = userId9,
                            CustomProperties = new Dictionary<string, string>
                            {
                                { "userId9", "The closer you get to the essence, the less confused you will be" }
                            }
                        }
                    }
                }, default);
                var reponse = await mediator.RequestAsync<GetMembersOfGroupRequest, SugarChatResponse<IEnumerable<GroupUserDto>>>(new GetMembersOfGroupRequest { UserId = userId, GroupId = conversationId });
                reponse.Data.First(x => x.UserId == userId).CustomProperties.Count().ShouldBe(3);
                reponse.Data.First(x => x.UserId == userId).CustomProperties.Count(x => x.Key == "Signature").ShouldBe(1);
                reponse.Data.First(x => x.UserId == userId).CustomProperties.Count(x => x.Key == "A" && x.Value == "2AB").ShouldBe(1);

                reponse = await mediator.RequestAsync<GetMembersOfGroupRequest, SugarChatResponse<IEnumerable<GroupUserDto>>>(new GetMembersOfGroupRequest { UserId = userId9, GroupId = groupId1 });
                reponse.Data.First(x => x.UserId == userId9).CustomProperties.Count().ShouldBe(3);
                reponse.Data.First(x => x.UserId == userId9).CustomProperties.Count(x => x.Key == "userId9").ShouldBe(1);
                reponse.Data.First(x => x.UserId == userId9).CustomProperties.Count(x => x.Key == "A" && x.Value == "0AB").ShouldBe(1);
            });
        }

        [Fact]
        public async Task ShouldGetMembersOfGroup()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
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
        public async Task ShouldCheckUserIsInGroup()
        {
            await Run<IMediator>(async (mediator) =>
            {
                {
                    var response = await mediator.SendAsync<CheckUserIsInGroupCommand, SugarChatResponse<bool>>(new CheckUserIsInGroupCommand { GroupId = Guid.NewGuid().ToString(), UserIds = new List<string> { Guid.NewGuid().ToString() } });
                    response.Data.ShouldBe(false);
                }
                {
                    var response = await mediator.SendAsync<CheckUserIsInGroupCommand, SugarChatResponse<bool>>(new CheckUserIsInGroupCommand { GroupId = conversationId, UserIds = new List<string> { userId2 } });
                    response.Data.ShouldBe(false);
                }
                {
                    var response = await mediator.SendAsync<CheckUserIsInGroupCommand, SugarChatResponse<bool>>(new CheckUserIsInGroupCommand { GroupId = conversationId, UserIds = new List<string> { userId } });
                    response.Data.ShouldBe(true);
                }
            });
        }
    }
}
