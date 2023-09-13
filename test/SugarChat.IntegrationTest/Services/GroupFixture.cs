using Mediator.Net;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Message.Exceptions;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Requests.Groups;
using SugarChat.Message.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using SugarChat.Message.Basic;
using SugarChat.Message.Common;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Commands.Messages;

namespace SugarChat.IntegrationTest.Services
{
    public class GroupFixture : TestBase
    {
        [Fact]
        public async Task ShouldAddGroup()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                AddGroupCommand command = new AddGroupCommand
                {
                    UserId = Guid.NewGuid().ToString(),
                    Id = Guid.NewGuid().ToString(),
                    CustomProperties = new Dictionary<string, string> { { "MerchId", "1" }, { "OrderId", "2" } },
                    CreatedBy = Guid.NewGuid().ToString(),
                    Type = 10
                };
                {
                    var response = await mediator.SendAsync<AddGroupCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.UserNoExists.WithParams(command.UserId).Message);
                }
                await repository.AddAsync(new User
                {
                    Id = command.UserId
                });
                await mediator.SendAsync<AddGroupCommand, SugarChatResponse>(command);
                var groupCustomProperties = await repository.ToListAsync<GroupCustomProperty>(x => x.GroupId == command.Id);
                groupCustomProperties.Any(x => x.Key == "MerchId" && x.Value == "1").ShouldBeTrue();
                groupCustomProperties.Any(x => x.Key == "OrderId" && x.Value == "2").ShouldBeTrue();
                var group = await repository.SingleAsync<Group>(x => x.Id == command.Id && x.CreatedBy == command.CreatedBy);
                group.Type.ShouldBe(10);
                (await repository.CountAsync<GroupUser>()).ShouldBe(1);
            });
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                var userId = Guid.NewGuid().ToString();
                await repository.AddAsync(new User { Id = userId });
                var groupId = Guid.NewGuid().ToString();
                await repository.AddAsync(new Group
                {
                    Id = groupId
                });
                var response = await mediator.SendAsync<AddGroupCommand, SugarChatResponse>(new AddGroupCommand { UserId = userId, Id = groupId });
                response.Code.ShouldBe(ExceptionCode.GroupExists);
                (await repository.CountAsync<GroupUser>(x => x.GroupId == groupId)).ShouldBe(0);
            });
        }

        [Fact]
        public async Task ShouldDismissGroup()
        {
            List<Group> groups = new List<Group>();
            for (int i = 0; i < 5; i++)
            {
                groups.Add(new Group
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = 1
                });
            }
            List<GroupUser> groupUsers = new List<GroupUser>();
            for (int i = 0; i < 10; i++)
            {
                var groupId = groups[i % 5].Id;
                groupUsers.Add(new GroupUser
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupId = groupId
                });
            }
            List<Core.Domain.Message> messages = new List<Core.Domain.Message>();
            for (int i = 0; i < 15; i++)
            {
                var groupId = groups[i % 5].Id;
                messages.Add(new Core.Domain.Message
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupId = groupId
                });
            }
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await repository.AddRangeAsync(groups);
                await repository.AddRangeAsync(groupUsers);
                await repository.AddRangeAsync(messages);

                DismissGroupCommand command = new DismissGroupCommand
                {
                    GroupId = Guid.NewGuid().ToString()
                };
                {
                    var response = await mediator.SendAsync<DismissGroupCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.GroupNoExists.WithParams(command.GroupId).Message);
                }

                command.GroupId = groups[0].Id;
                await mediator.SendAsync(command);
                (await repository.AnyAsync<Group>(x => x.Id == command.GroupId)).ShouldBeFalse();
                (await repository.AnyAsync<GroupUser>(x => x.Id == command.GroupId)).ShouldBeFalse();
                (await repository.AnyAsync<Core.Domain.Message>(x => x.Id == command.GroupId)).ShouldBeFalse();
                (await repository.CountAsync<Group>(x => x.Type == 1)).ShouldBe(4);
                (await repository.CountAsync<GroupUser>()).ShouldBe(8);
                (await repository.CountAsync<Core.Domain.Message>()).ShouldBe(12);
            });
        }

        [Fact]
        public async Task ShouldGetByCustomProperties()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                string[] userIds = new string[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
                for (int j = 0; j < 2; j++)
                {
                    await repository.AddAsync(new User
                    {
                        Id = userIds[j]
                    });
                    for (int i = 0; i < 3; i++)
                    {
                        string groupId = Guid.NewGuid().ToString();
                        await repository.AddAsync(new GroupUser
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = userIds[j],
                            GroupId = groupId
                        });
                        await repository.AddAsync(new Group
                        {
                            Id = groupId,
                            Type = 2
                        });
                        await repository.AddAsync(new GroupCustomProperty
                        {
                            GroupId = groupId,
                            Key = "merchId",
                            Value = $"a{i + 1}{i + 1}"
                        });
                        await repository.AddAsync(new GroupCustomProperty
                        {
                            GroupId = groupId,
                            Key = "userId",
                            Value = $"b{i + 1}{i + 2}"
                        });
                    }
                }
                {
                    var request = new GetGroupByCustomPropertiesRequest()
                    {
                        UserId = Guid.NewGuid().ToString(),
                        GroupType = 2
                    };
                    var response = await mediator.RequestAsync<GetGroupByCustomPropertiesRequest, SugarChatResponse<IEnumerable<GroupDto>>>(request);
                    response.Message.ShouldBe(Prompt.UserNoExists.WithParams(request.UserId).Message);
                }
                {
                    var response = await mediator.RequestAsync<GetGroupByCustomPropertiesRequest, SugarChatResponse<IEnumerable<GroupDto>>>(new GetGroupByCustomPropertiesRequest()
                    {
                        UserId = userIds[0],
                        CustomProperties = new Dictionary<string, string> { { "merchId", "a11" }, { "userId", "b12" } },
                        GroupType = 2
                    });
                    response.Data.Count().ShouldBe(1);
                    response.Data.First().Type.ShouldBe(2);
                }
                {
                    var response = await mediator.RequestAsync<GetGroupByCustomPropertiesRequest, SugarChatResponse<IEnumerable<GroupDto>>>(new GetGroupByCustomPropertiesRequest()
                    {
                        UserId = userIds[0],
                        CustomProperties = new Dictionary<string, string> { { "merchId", "a11" }, { "userId", "b12" } },
                        SearchAllGroup = true,
                        GroupType = 2
                    });
                    response.Data.Count().ShouldBe(2);
                    response.Data.Count(x => x.Type == 2).ShouldBe(2);
                }
            });
        }

        [Fact]
        public async Task ShouldBatchAddGroup()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                var userIds = new List<string>();
                for (int i = 0; i < 10; i++)
                {
                    string userId = Guid.NewGuid().ToString();
                    await mediator.SendAsync(new AddUserCommand { Id = userId });
                    userIds.Add(userId);
                }

                var addUserCommand = new AddUserCommand { Id = Guid.NewGuid().ToString() };
                await mediator.SendAsync(addUserCommand);
                var addGroupCommands = new List<AddGroupCommand>();
                for (int i = 0; i < 10; i++)
                {
                    addGroupCommands.Add(new AddGroupCommand
                    {
                        Id = "Id" + i,
                        Name = "Name" + i,
                        AvatarUrl = "AvatarUrl" + i,
                        Description = "Description" + i,
                        CustomProperties = new Dictionary<string, string> { { "CustomProperties" + i, i.ToString() } },
                        Type = i
                    });
                }
                await mediator.SendAsync(new BatchAddGroupCommand { UserId = addUserCommand.Id, AddGroupCommands = addGroupCommands });
                var groups = await repository.ToListAsync<Group>();
                groups.Count().ShouldBe(10);
                for (int i = 0; i < 10; i++)
                {
                    var group = groups.Single(x => x.Id == "Id" + i);
                    group.Name.ShouldBe("Name" + i);
                    group.AvatarUrl.ShouldBe("AvatarUrl" + i);
                    group.Description.ShouldBe("Description" + i);
                    group.CustomProperties.ShouldBe(new Dictionary<string, string> { { "CustomProperties" + i, i.ToString() } });
                    group.Type.ShouldBe(i);
                }
                await ShouldBatchAddGroupMember(addUserCommand.Id, userIds, groups.Select(x => x.Id).ToList());
                await ShouldBatchSaveMessage(addUserCommand.Id, userIds, groups.Select(x => x.Id).ToList());
            });
        }

        private async Task ShouldBatchAddGroupMember(string userId, IEnumerable<string> userIds, IEnumerable<string> groupIds)
        {
            var addGroupMemberCommands = new List<AddGroupMemberCommand>();
            for (int i = 0; i < 10; i++)
            {
                addGroupMemberCommands.Add(new AddGroupMemberCommand
                {
                    GroupId = groupIds.ElementAt(i),
                    AdminId = userId,
                    GroupUserIds = userIds,
                    Role = Message.UserRole.Owner,
                    CustomProperties = new Dictionary<string, string> { { "GroupUserCustomProperties" + i, i.ToString() } }
                });
            }
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await mediator.SendAsync(new BatchAddGroupMemberCommand { UserId = userId, AddGroupMemberCommands = addGroupMemberCommands });
                var groups = await repository.ToListAsync<Group>();
                groups.Count().ShouldBe(10);
                var groupUsers = await repository.ToListAsync<GroupUser>();
                groupUsers.Count().ShouldBe(110);
                for (int i = 0; i < 10; i++)
                {
                    var _groupUsers = groupUsers.Where(x => x.GroupId == groups[i].Id && x.UserId != userId).ToList();
                    _groupUsers.Count().ShouldBe(10);
                    foreach (var groupUser in _groupUsers)
                    {
                        groupUser.CustomProperties.Count().ShouldBe(2);
                        groupUser.CustomProperties.GetValueOrDefault("CustomProperties" + i).ShouldBe(i.ToString());
                        groupUser.CustomProperties.GetValueOrDefault("GroupUserCustomProperties" + i).ShouldBe(i.ToString());
                    }
                }
            });
        }

        private async Task ShouldBatchSaveMessage(string userId, IEnumerable<string> userIds, IEnumerable<string> groupIds)
        {
            var sendMessageCommands = new List<SendMessageCommand>();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    sendMessageCommands.Add(new SendMessageCommand
                    {
                        Id = "Id" + i + j,
                        GroupId = groupIds.ElementAt(i),
                        Content = i.ToString() + j,
                        Type = i * j,
                        SentBy = userIds.ElementAt(j),
                        IsSystem = true,
                        Payload = i.ToString() + j,
                        CustomProperties = new Dictionary<string, string> { { "MessageCustomProperties" + i, i.ToString() + j } }
                    });
                }
            }
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await mediator.SendAsync(new BatchSendMessageCommand { UserId = userId, SendMessageCommands = sendMessageCommands });
                var messages = await repository.ToListAsync<Core.Domain.Message>();
                messages.Count().ShouldBe(100);
                var groupUsers = await repository.ToListAsync<GroupUser>();
                groupUsers.Count().ShouldBe(110);
                for (int i = 0; i < 10; i++)
                {
                    messages.Count(x => x.GroupId == groupIds.ElementAt(i)).ShouldBe(10);
                    messages.Select(x => x.SentBy).Distinct().Count().ShouldBe(10);
                    groupUsers.Count(x => x.GroupId == groupIds.ElementAt(i)).ShouldBe(11);
                    groupUsers.Count(x => x.GroupId == groupIds.ElementAt(i) && x.UserId == userId && x.UnreadCount == 10).ShouldBe(1);
                    groupUsers.Count(x => x.GroupId == groupIds.ElementAt(i) && x.UserId != userId && x.UnreadCount == 9).ShouldBe(10);
                    for (int j = 0; j < 10; j++)
                    {
                        var message = messages.Single(x => x.Id == "Id" + i + j);
                        message.GroupId.ShouldBe(groupIds.ElementAt(i));
                        message.Content.ShouldBe(i.ToString() + j);
                        message.Type.ShouldBe(i * j);
                        message.SentBy.ShouldBe(userIds.ElementAt(j));
                        message.IsSystem.ShouldBe(true);
                        message.Payload.ShouldBe(i.ToString() + j);
                        message.CustomProperties.Count().ShouldBe(1);
                        message.CustomProperties.GetValueOrDefault("MessageCustomProperties" + i).ShouldBe(i.ToString() + j);
                    }
                }
            });
        }
    }
}