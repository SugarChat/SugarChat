using Mediator.Net;
using Shouldly;
using SugarChat.Core.Basic;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Services;
using SugarChat.Message;
using SugarChat.Message.Commands.GroupUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.IntegrationTest.Services
{
    public class GroupUserFixture : TestBase
    {
        private string groupId = Guid.NewGuid().ToString();
        private string groupOwnerId = Guid.NewGuid().ToString();

        private string[] groupAdminIds = new string[] {Guid.NewGuid().ToString(), Guid.NewGuid().ToString()};

        private string[] userIds = new string[]
            {Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString()};

        private List<Func<IRepository, Task>> addGroupAdminFunctions = new();
        private List<Func<int, IRepository, Task>> addGroupMemberFunctions = new();
        private List<Func<IRepository, Task>> addUserFunctions = new();

        public GroupUserFixture()
        {
            foreach (var groupAdminId in groupAdminIds)
            {
                addGroupAdminFunctions.Add(async (repository) =>
                {
                    await repository.AddAsync(new User
                    {
                        Id = groupAdminId
                    });
                    await repository.AddAsync(new GroupUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = groupAdminId,
                        GroupId = groupId,
                        Role = UserRole.Admin
                    });
                });
            }

            foreach (var userId in userIds)
            {
                addUserFunctions.Add(async (repository) =>
                {
                    await repository.AddAsync(new User
                    {
                        Id = userId
                    });
                });
            }

            for (int i = 0; i < userIds.Length; i++)
            {
                addGroupMemberFunctions.Add(async (index, repository) =>
                {
                    await addUserFunctions[index].Invoke(repository);
                    await repository.AddAsync(new GroupUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = userIds[index],
                        GroupId = groupId
                    });
                });
            }
        }

        private async Task AddGroup(IRepository repository)
        {
            await repository.AddAsync(new Group
            {
                Id = groupId,
                Name = "testGroup",
                AvatarUrl = "testAvatarUrl",
                Description = "testDescription"
            });
        }

        private async Task AddGroupOwner(IRepository repository)
        {
            await repository.AddAsync(new User
            {
                Id = groupOwnerId
            });
            await repository.AddAsync(new GroupUser
            {
                Id = Guid.NewGuid().ToString(),
                UserId = groupOwnerId,
                GroupId = groupId,
                Role = UserRole.Owner
            });
        }

        [Fact]
        public async Task ShouldJoinGroup()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await AddGroup(repository);
                await addUserFunctions[0].Invoke(repository);
                JoinGroupCommand command = new JoinGroupCommand
                {
                    GroupId = Guid.NewGuid().ToString(),
                    UserId = Guid.NewGuid().ToString(),
                };
                {
                    var response = await mediator.SendAsync<JoinGroupCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.GroupNoExists.WithParams(command.GroupId).Message);
                }
                {
                    command.GroupId = groupId;
                    var response = await mediator.SendAsync<JoinGroupCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.UserNoExists.WithParams(command.UserId).Message);
                }

                command.UserId = userIds[0];
                await mediator.SendAsync(command);
                (await repository.AnyAsync<GroupUser>(x => x.GroupId == command.GroupId && x.UserId == command.UserId))
                    .ShouldBeTrue();

                {
                    var response = await mediator.SendAsync<JoinGroupCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(
                        Prompt.GroupUserExists.WithParams(command.UserId, command.GroupId).Message);
                }
            });
        }

        [Fact]
        public async Task ShouldQuitGroup()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await AddGroup(repository);
                await AddGroupOwner(repository);
                await addGroupMemberFunctions[0].Invoke(0, repository);
                QuitGroupCommand command = new QuitGroupCommand
                {
                    GroupId = Guid.NewGuid().ToString(),
                    UserId = Guid.NewGuid().ToString()
                };
                {
                    var response = await mediator.SendAsync<QuitGroupCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.GroupNoExists.WithParams(command.GroupId).Message);
                }
                {
                    command.GroupId = groupId;
                    var response = await mediator.SendAsync<QuitGroupCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.UserNoExists.WithParams(command.UserId).Message);
                }
                {
                    command.UserId = groupOwnerId;
                    var response = await mediator.SendAsync<QuitGroupCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.IsOwner.WithParams(command.UserId, command.GroupId).Message);
                }

                command.UserId = userIds[0];
                await mediator.SendAsync(command);
                (await repository.ToListAsync<GroupUser>()).Count.ShouldBe(1);
                (await repository.ToListAsync<GroupUser>(x => x.GroupId == command.GroupId && x.UserId == groupOwnerId))
                    .Count.ShouldBe(1);

                {
                    var response = await mediator.SendAsync<QuitGroupCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.NotInGroup.WithParams(command.UserId, command.GroupId).Message);
                }
            });
        }

        [Fact]
        public async Task ShouldChangeGroupOwner()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await AddGroup(repository);
                await AddGroupOwner(repository);
                await addGroupMemberFunctions[0].Invoke(0, repository);
                ChangeGroupOwnerCommand command = new ChangeGroupOwnerCommand
                {
                    OwnerId = Guid.NewGuid().ToString(),
                    NewOwnerId = Guid.NewGuid().ToString(),
                    GroupId = Guid.NewGuid().ToString()
                };
                {
                    var response = await mediator.SendAsync<ChangeGroupOwnerCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.NotInGroup.WithParams(command.OwnerId, command.GroupId).Message);
                }
                {
                    command.GroupId = groupId;
                    var response = await mediator.SendAsync<ChangeGroupOwnerCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.NotInGroup.WithParams(command.OwnerId, command.GroupId).Message);
                }
                {
                    command.OwnerId = userIds[0];
                    var response = await mediator.SendAsync<ChangeGroupOwnerCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.IsNotOwner.WithParams(command.OwnerId, command.GroupId).Message);
                }
                {
                    command.OwnerId = groupOwnerId;
                    var response = await mediator.SendAsync<ChangeGroupOwnerCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.NotInGroup.WithParams(command.NewOwnerId, command.GroupId)
                        .Message);
                }

                command.NewOwnerId = userIds[0];
                await mediator.SendAsync(command);

                (await repository.FirstOrDefaultAsync<GroupUser>(x =>
                    x.GroupId == command.GroupId && x.UserId == command.OwnerId)).Role.ShouldBe(UserRole.Admin);

                (await repository.FirstOrDefaultAsync<GroupUser>(x =>
                    x.GroupId == command.GroupId && x.UserId == command.NewOwnerId)).Role.ShouldBe(UserRole.Owner);
            });
        }

        [Fact]
        public async Task ShouldAddGroupMember()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await AddGroup(repository);
                await AddGroupOwner(repository);
                await addGroupAdminFunctions[0].Invoke(repository);
                addUserFunctions.ForEach(action => action(repository));
                AddGroupMemberCommand command = new AddGroupMemberCommand
                {
                    GroupId = Guid.NewGuid().ToString(),
                    AdminId = Guid.NewGuid().ToString(),
                    UserIdList = new List<string> {Guid.NewGuid().ToString(), Guid.NewGuid().ToString()}
                };
                {
                    var response = await mediator.SendAsync<AddGroupMemberCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.NotInGroup.WithParams(command.AdminId, command.GroupId).Message);
                }
                {
                    command.GroupId = groupId;
                    var response = await mediator.SendAsync<AddGroupMemberCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.NotInGroup.WithParams(command.AdminId, command.GroupId).Message);
                }
                {
                    command.AdminId = groupAdminIds[0];
                    var response = await mediator.SendAsync<AddGroupMemberCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.NotAllUsersExists.Message);
                }

                command.UserIdList = userIds.Take(2).ToList();
                await mediator.SendAsync(command);
                (await repository.CountAsync<GroupUser>(x =>
                    x.GroupId == command.GroupId && command.UserIdList.Contains(x.UserId))).ShouldBe(2);

                command.AdminId = groupOwnerId;
                command.UserIdList = new List<string> {userIds[2]};
                await mediator.SendAsync(command);
                (await repository.CountAsync<GroupUser>(x =>
                    x.GroupId == command.GroupId && command.UserIdList.Contains(x.UserId))).ShouldBe(1);

                {
                    var response = await mediator.SendAsync<AddGroupMemberCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.SomeGroupUsersExist.Message);
                }
            });
        }

        [Fact]
        public async Task ShouldDeleteGroupMember()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await AddGroup(repository);
                await AddGroupOwner(repository);
                await addGroupAdminFunctions[0].Invoke(repository);
                await addGroupAdminFunctions[1].Invoke(repository);
                for (int i = 0; i < userIds.Length; i++)
                {
                    await addGroupMemberFunctions[i].Invoke(i, repository);
                }

                DeleteGroupMemberCommand command = new DeleteGroupMemberCommand
                {
                    GroupId = Guid.NewGuid().ToString(),
                    AdminId = Guid.NewGuid().ToString(),
                    UserIdList = new List<string> {Guid.NewGuid().ToString(), Guid.NewGuid().ToString()}
                };
                {
                    var response = await mediator.SendAsync<DeleteGroupMemberCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.NotInGroup.WithParams(command.AdminId, command.GroupId).Message);
                }
                {
                    command.GroupId = groupId;
                    var response = await mediator.SendAsync<DeleteGroupMemberCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.NotInGroup.WithParams(command.AdminId, command.GroupId).Message);
                }
                {
                    command.AdminId = groupAdminIds[0];
                    command.UserIdList = new List<string> {groupAdminIds[1]};
                    var response = await mediator.SendAsync<DeleteGroupMemberCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe($"amdin can't delete amdin with Id {command.UserIdList[0]}.");
                }
                {
                    command.UserIdList = new List<string> {groupOwnerId};
                    var response = await mediator.SendAsync<DeleteGroupMemberCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe($"the deleted member cannot be owner with Id {command.UserIdList[0]}.");
                }

                command.UserIdList = userIds.Take(2).ToList();
                await mediator.SendAsync(command);
                (await repository.AnyAsync<GroupUser>(x =>
                    x.GroupId == command.GroupId && command.UserIdList.Contains(x.UserId))).ShouldBeFalse();

                {
                    var response = await mediator.SendAsync<DeleteGroupMemberCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.NotInGroup.WithParams(command.UserIdList[0], command.GroupId)
                        .Message);
                }
            });
        }

        [Fact]
        public async Task SetMessageRemindType()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await AddGroup(repository);
                await addGroupMemberFunctions[0].Invoke(0, repository);

                SetMessageRemindTypeCommand command = new SetMessageRemindTypeCommand
                {
                    GroupId = Guid.NewGuid().ToString(),
                    UserId = Guid.NewGuid().ToString(),
                    MessageRemindType = default
                };
                {
                    var response = await mediator.SendAsync<SetMessageRemindTypeCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.NotInGroup.WithParams(command.UserId, command.GroupId).Message);
                }
                {
                    command.GroupId = groupId;
                    var response = await mediator.SendAsync<SetMessageRemindTypeCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.NotInGroup.WithParams(command.UserId, command.GroupId).Message);
                }

                command.UserId = userIds[0];
                await mediator.SendAsync(command);
                (await repository.AnyAsync<GroupUser>(x =>
                    x.GroupId == command.GroupId && x.UserId == command.UserId &&
                    x.MessageRemindType == command.MessageRemindType)).ShouldBeTrue();

                command.MessageRemindType = MessageRemindType.DISCARD;
                await mediator.SendAsync(command);
                (await repository.AnyAsync<GroupUser>(x =>
                    x.GroupId == command.GroupId && x.UserId == command.UserId &&
                    x.MessageRemindType == command.MessageRemindType)).ShouldBeTrue();
            });
        }

        [Fact]
        public async Task ShouldSetGroupMemberRole()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await AddGroup(repository);
                await AddGroupOwner(repository);
                await addGroupMemberFunctions[0].Invoke(0, repository);

                SetGroupMemberRoleCommand command = new SetGroupMemberRoleCommand
                {
                    GroupId = Guid.NewGuid().ToString(),
                    OwnerId = Guid.NewGuid().ToString(),
                    MemberId = Guid.NewGuid().ToString(),
                    Role = UserRole.Admin
                };
                {
                    var response = await mediator.SendAsync<SetGroupMemberRoleCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.NotInGroup.WithParams(command.OwnerId, command.GroupId).Message);
                }
                {
                    command.GroupId = groupId;
                    var response = await mediator.SendAsync<SetGroupMemberRoleCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.NotInGroup.WithParams(command.OwnerId, command.GroupId).Message);
                }
                {
                    command.OwnerId = userIds[0];
                    var response = await mediator.SendAsync<SetGroupMemberRoleCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.IsNotOwner.WithParams(command.OwnerId, command.GroupId).Message);
                }
                {
                    command.OwnerId = groupOwnerId;
                    var response = await mediator.SendAsync<SetGroupMemberRoleCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.NotInGroup.WithParams(command.MemberId, command.GroupId).Message);
                }

                command.MemberId = userIds[0];
                await mediator.SendAsync(command);
                (await repository.AnyAsync<GroupUser>(x =>
                        x.GroupId == command.GroupId && x.UserId == command.MemberId && x.Role == command.Role))
                    .ShouldBeTrue();

                {
                    command.Role = UserRole.Owner;
                    var response = await mediator.SendAsync<SetGroupMemberRoleCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe($"can't set group member role to owner with Id {command.MemberId}.");
                }
            });
        }
    }
}