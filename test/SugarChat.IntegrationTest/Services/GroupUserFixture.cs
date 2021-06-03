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
        private string groupAdmin1Id = Guid.NewGuid().ToString();
        private string groupAdmin2Id = Guid.NewGuid().ToString();
        private string userId = Guid.NewGuid().ToString();

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

        private async Task AddGroupAdmin1(IRepository repository)
        {
            await repository.AddAsync(new User
            {
                Id = groupAdmin1Id
            });
            await repository.AddAsync(new GroupUser
            {
                Id = Guid.NewGuid().ToString(),
                UserId = groupAdmin1Id,
                GroupId = groupId,
                Role = UserRole.Admin
            });
        }

        private async Task AddGroupAdmin2(IRepository repository)
        {
            await repository.AddAsync(new User
            {
                Id = groupAdmin2Id
            });
            await repository.AddAsync(new GroupUser
            {
                Id = Guid.NewGuid().ToString(),
                UserId = groupAdmin2Id,
                GroupId = groupId,
                Role = UserRole.Admin
            });
        }

        private async Task AddUser(IRepository repository)
        {
            await repository.AddAsync(new User
            {
                Id = userId
            });
        }

        private async Task AddGroupUser(IRepository repository)
        {
            await AddUser(repository);
            await repository.AddAsync(new GroupUser
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                GroupId = groupId
            });
        }

        [Fact]
        public async Task ShouldJoinGroup()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await AddGroup(repository);
                await AddUser(repository);
                JoinGroupCommand command = new JoinGroupCommand
                {
                    GroupId = Guid.NewGuid().ToString(),
                    UserId = Guid.NewGuid().ToString(),
                };
                {
                    var response = await mediator.SendAsync<JoinGroupCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.GroupNoExists, command.GroupId));
                }
                {
                    command.GroupId = groupId;
                    var response = await mediator.SendAsync<JoinGroupCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.UserNoExists, command.UserId));
                }

                command.UserId = userId;
                await mediator.SendAsync(command);
                (await repository.AnyAsync<GroupUser>(x => x.GroupId == command.GroupId && x.UserId == command.UserId)).ShouldBeTrue();

                {
                    var response = await mediator.SendAsync<JoinGroupCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.InGroup, command.UserId, command.GroupId));
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
                await AddGroupUser(repository);
                QuitGroupCommand command = new QuitGroupCommand
                {
                    GroupId = Guid.NewGuid().ToString(),
                    UserId = Guid.NewGuid().ToString()
                };
                {
                    var response = await mediator.SendAsync<QuitGroupCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.NotInGroup, command.UserId, command.GroupId));
                }
                {
                    command.GroupId = groupId;
                    var response = await mediator.SendAsync<QuitGroupCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.NotInGroup, command.UserId, command.GroupId));
                }
                {
                    command.UserId = groupOwnerId;
                    var response = await mediator.SendAsync<QuitGroupCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.IsOwner, command.UserId, command.GroupId));
                }

                command.UserId = userId;
                await mediator.SendAsync(command);
                (await repository.ToListAsync<GroupUser>()).Count.ShouldBe(1);
                (await repository.ToListAsync<GroupUser>(x => x.GroupId == command.GroupId && x.UserId == groupOwnerId)).Count.ShouldBe(1);

                {
                    var response = await mediator.SendAsync<QuitGroupCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.NotInGroup, command.UserId, command.GroupId));
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
                await AddGroupUser(repository);
                ChangeGroupOwnerCommand command = new ChangeGroupOwnerCommand
                {
                    FromUserId = Guid.NewGuid().ToString(),
                    ToUserId = Guid.NewGuid().ToString(),
                    GroupId = Guid.NewGuid().ToString()
                };
                {
                    var response = await mediator.SendAsync<ChangeGroupOwnerCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.NotInGroup, command.FromUserId, command.GroupId));
                }
                {
                    command.GroupId = groupId;
                    var response = await mediator.SendAsync<ChangeGroupOwnerCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.NotInGroup, command.FromUserId, command.GroupId));
                }
                {
                    command.FromUserId = userId;
                    var response = await mediator.SendAsync<ChangeGroupOwnerCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.IsNotOwner, command.FromUserId, command.GroupId));
                }
                {
                    command.FromUserId = groupOwnerId;
                    var response = await mediator.SendAsync<ChangeGroupOwnerCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.NotInGroup, command.ToUserId, command.GroupId));
                }

                command.ToUserId = userId;
                await mediator.SendAsync(command);

                (await repository.FirstOrDefaultAsync<GroupUser>(x => x.GroupId == command.GroupId && x.UserId == command.FromUserId)).Role.ShouldBe(UserRole.Admin);

                (await repository.FirstOrDefaultAsync<GroupUser>(x => x.GroupId == command.GroupId && x.UserId == command.ToUserId)).Role.ShouldBe(UserRole.Owner);
            });
        }

        [Fact]
        public async Task ShouldAddGroupMember()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await AddGroup(repository);
                await AddGroupOwner(repository);
                await AddGroupAdmin1(repository);
                AddGroupMemberCommand command = new AddGroupMemberCommand
                {
                    GroupId = Guid.NewGuid().ToString(),
                    AdminId = Guid.NewGuid().ToString(),
                    MemberId = Guid.NewGuid().ToString()
                };
                {
                    var response = await mediator.SendAsync<AddGroupMemberCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.NotInGroup, command.AdminId, command.GroupId));
                }
                {
                    command.GroupId = groupId;
                    var response = await mediator.SendAsync<AddGroupMemberCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.NotInGroup, command.AdminId, command.GroupId));
                }

                command.AdminId = groupAdmin1Id;
                await mediator.SendAsync(command);
                (await repository.AnyAsync<GroupUser>(x => x.GroupId == command.GroupId && x.UserId == command.MemberId)).ShouldBeTrue();

                command.AdminId = groupOwnerId;
                command.MemberId = Guid.NewGuid().ToString();
                await mediator.SendAsync(command);
                (await repository.AnyAsync<GroupUser>(x => x.GroupId == command.GroupId && x.UserId == command.MemberId)).ShouldBeTrue();

                command.MemberId = userId;
                await mediator.SendAsync(command);
                (await repository.AnyAsync<GroupUser>(x => x.GroupId == command.GroupId && x.UserId == command.MemberId)).ShouldBeTrue();

                {
                    var response = await mediator.SendAsync<AddGroupMemberCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.InGroup, command.MemberId, command.GroupId));
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
                await AddGroupAdmin1(repository);
                await AddGroupAdmin2(repository);
                await AddGroupUser(repository);
                DeleteGroupMemberCommand command = new DeleteGroupMemberCommand
                {
                    GroupId = Guid.NewGuid().ToString(),
                    AdminId = Guid.NewGuid().ToString(),
                    MemberId = Guid.NewGuid().ToString()
                };
                {
                    var response = await mediator.SendAsync<DeleteGroupMemberCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.NotInGroup, command.AdminId, command.GroupId));
                }
                {
                    command.GroupId = groupId;
                    var response = await mediator.SendAsync<DeleteGroupMemberCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.NotInGroup, command.AdminId, command.GroupId));
                }
                {
                    command.AdminId = groupAdmin1Id;
                    command.MemberId = groupAdmin2Id;
                    var response = await mediator.SendAsync<DeleteGroupMemberCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe("amdin can't delete amdin.");
                }
                {
                    command.MemberId = groupOwnerId;
                    var response = await mediator.SendAsync<DeleteGroupMemberCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe("the deleted member cannot be owner.");
                }

                command.MemberId = userId;
                await mediator.SendAsync(command);
                (await repository.AnyAsync<GroupUser>(x => x.GroupId == command.GroupId && x.UserId == command.MemberId)).ShouldBeFalse();

                {
                    var response = await mediator.SendAsync<DeleteGroupMemberCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.NotInGroup, command.MemberId, command.GroupId));
                }
            });
        }

        [Fact]
        public async Task SetMessageRemindType()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await AddGroup(repository);
                await AddGroupUser(repository);

                SetMessageRemindTypeCommand command = new SetMessageRemindTypeCommand
                {
                    GroupId = Guid.NewGuid().ToString(),
                    UserId = Guid.NewGuid().ToString(),
                    MessageRemindType = default
                };
                {
                    var response = await mediator.SendAsync<SetMessageRemindTypeCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.NotInGroup, command.UserId, command.GroupId));
                }
                {
                    command.GroupId = groupId;
                    var response = await mediator.SendAsync<SetMessageRemindTypeCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.NotInGroup, command.UserId, command.GroupId));
                }

                command.UserId = userId;
                await mediator.SendAsync(command);
                (await repository.AnyAsync<GroupUser>(x => x.GroupId == command.GroupId && x.UserId == command.UserId && x.MessageRemindType == command.MessageRemindType)).ShouldBeTrue();

                command.MessageRemindType = MessageRemindType.DISCARD;
                await mediator.SendAsync(command);
                (await repository.AnyAsync<GroupUser>(x => x.GroupId == command.GroupId && x.UserId == command.UserId && x.MessageRemindType == command.MessageRemindType)).ShouldBeTrue();
            });
        }

        [Fact]
        public async Task ShouldSetGroupMemberRole()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await AddGroup(repository);
                await AddGroupOwner(repository);
                await AddGroupUser(repository);

                SetGroupMemberRoleCommand command = new SetGroupMemberRoleCommand
                {
                    GroupId = Guid.NewGuid().ToString(),
                    OwnerId = Guid.NewGuid().ToString(),
                    MemberId = Guid.NewGuid().ToString(),
                    Role = UserRole.Admin
                };
                {
                    var response = await mediator.SendAsync<SetGroupMemberRoleCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.NotInGroup, command.OwnerId, command.GroupId));
                }
                {
                    command.GroupId = groupId;
                    var response = await mediator.SendAsync<SetGroupMemberRoleCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.NotInGroup, command.OwnerId, command.GroupId));
                }
                {
                    command.OwnerId = userId;
                    var response = await mediator.SendAsync<SetGroupMemberRoleCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.IsNotOwner, command.OwnerId, command.GroupId));
                }
                {
                    command.OwnerId = groupOwnerId;
                    var response = await mediator.SendAsync<SetGroupMemberRoleCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.NotInGroup, command.MemberId, command.GroupId));
                }

                command.MemberId = userId;
                await mediator.SendAsync(command);
                (await repository.AnyAsync<GroupUser>(x => x.GroupId == command.GroupId && x.UserId == command.MemberId && x.Role == command.Role)).ShouldBeTrue();

                {
                    command.Role = UserRole.Owner;
                    var response = await mediator.SendAsync<SetGroupMemberRoleCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe("can't set group member role to owner.");
                }
            });
        }
    }
}