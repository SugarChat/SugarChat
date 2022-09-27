using Mediator.Net;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Message.Exceptions;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Services;
using SugarChat.Message;
using SugarChat.Message.Commands.GroupUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SugarChat.Message.Dtos;
using Xunit;
using SugarChat.Message.Basic;
using AutoMapper;
using SugarChat.Message.Dtos.GroupUsers;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using Autofac;
using NSubstitute;

namespace SugarChat.IntegrationTest.Services
{
    public class GroupUserFixture : TestBase
    {
        private string groupId = Guid.NewGuid().ToString();
        private string groupOwnerId = Guid.NewGuid().ToString();

        private string[] groupAdminIds = new string[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };

        private string[] userIds = new string[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };

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
                    var groupUser = new GroupUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = groupAdminId,
                        GroupId = groupId,
                        Role = UserRole.Admin
                    };
                    await repository.AddAsync(groupUser);
                    await repository.AddAsync(new GroupUserCustomProperty
                    {
                        GroupUserId = groupUser.Id,
                        Key = "Role",
                        Value = "Admin"
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
                    var groupUser = new GroupUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = userIds[index],
                        GroupId = groupId
                    };
                    await repository.AddAsync(groupUser);
                    await repository.AddAsync(new GroupUserCustomProperty
                    {
                        GroupUserId = groupUser.Id,
                        Key = "Role",
                        Value = "Admin"
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
                    GroupId = groupId,
                    AdminId = Guid.NewGuid().ToString(),
                    GroupUserIds = new string[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() },
                    CustomProperties = new Dictionary<string, string> { { "UserType", "Member" }, { "Number", "1" } },
                    CreatedBy = Guid.NewGuid().ToString()
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

                command.GroupUserIds = userIds.Take(2).ToList();
                await mediator.SendAsync(command);
                (await repository.CountAsync<GroupUser>(x => x.GroupId == command.GroupId && command.GroupUserIds.Contains(x.UserId) && x.CreatedBy == command.CreatedBy)).ShouldBe(2);
                (await repository.CountAsync<GroupUserCustomProperty>(x => command.GroupUserIds.Contains(x.GroupUserId) && x.CreatedBy == command.CreatedBy)).ShouldBe(4);

                command.AdminId = groupOwnerId;
                command.GroupUserIds = new string[] { userIds[1] };
                await mediator.SendAsync(command);
                (await repository.CountAsync<GroupUser>(x => x.GroupId == command.GroupId && command.GroupUserIds.Contains(x.UserId) && x.CreatedBy == command.CreatedBy)).ShouldBe(1);
                {
                    var response = await mediator.SendAsync<AddGroupMemberCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.SomeGroupUsersExist.Message);
                    (await repository.CountAsync<GroupUserCustomProperty>(x => command.GroupUserIds.Contains(x.GroupUserId) && x.CreatedBy == command.CreatedBy)).ShouldBe(2);
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

                (await repository.CountAsync<GroupUserCustomProperty>()).ShouldBe(5);
                RemoveGroupMemberCommand command = new RemoveGroupMemberCommand
                {
                    GroupId = Guid.NewGuid().ToString(),
                    AdminId = Guid.NewGuid().ToString(),
                    UserIdList = new List<string> { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() }
                };
                {
                    var response = await mediator.SendAsync<RemoveGroupMemberCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.NotInGroup.WithParams(command.AdminId, command.GroupId).Message);
                }
                {
                    command.GroupId = groupId;
                    var response = await mediator.SendAsync<RemoveGroupMemberCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.NotInGroup.WithParams(command.AdminId, command.GroupId).Message);
                }
                {
                    command.AdminId = groupAdminIds[0];
                    command.UserIdList = new List<string> { groupAdminIds[1] };
                    var response = await mediator.SendAsync<RemoveGroupMemberCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.RemoveAdminByAdmin.Message);
                }
                {
                    command.UserIdList = new List<string> { groupOwnerId };
                    var response = await mediator.SendAsync<RemoveGroupMemberCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.RemoveOwnerFromGroup.Message);
                }

                command.UserIdList = userIds.Take(2).ToList();
                await mediator.SendAsync(command);
                (await repository.AnyAsync<GroupUser>(x => x.GroupId == command.GroupId && command.UserIdList.Contains(x.UserId))).ShouldBeFalse();
                (await repository.CountAsync<GroupUserCustomProperty>()).ShouldBe(3);

                {
                    var response = await mediator.SendAsync<RemoveGroupMemberCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.NotAllGroupUsersExist.Message);
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
                    response.Message.ShouldBe(Prompt.SetGroupOwner.Message);
                }
            });
        }

        [Fact]
        public async Task ShouldUpdateGroupUserData()
        {
            await Run<IMediator, IRepository, IMapper>(async (mediator, repository, mapper) =>
            {
                var userId = Guid.NewGuid().ToString();
                var groupId2 = Guid.NewGuid().ToString();
                await repository.AddAsync(new User
                {
                    Id = userId
                });
                await AddGroup(repository);
                await repository.AddAsync(new Group
                {
                    Id = groupId2,
                    Name = "testGroup2",
                    AvatarUrl = "testAvatarUrl2",
                    Description = "testDescription2"
                });
                AddGroupMemberCommand command = new AddGroupMemberCommand
                {
                    GroupId = groupId,
                    AdminId = Guid.NewGuid().ToString(),
                    GroupUserIds = userIds,
                    CreatedBy = userId,
                    Role = UserRole.Admin
                };
                await mediator.SendAsync<AddGroupMemberCommand, SugarChatResponse>(command);
                var groupUsers = await repository.ToListAsync<GroupUser>();
                var groupUesrDtos = mapper.Map<IEnumerable<GroupUserDto>>(groupUsers);
                foreach (var groupUesrDto in groupUesrDtos)
                {
                    groupUesrDto.UserId = userIds[0];
                    groupUesrDto.GroupId = groupId2;
                    groupUesrDto.LastReadTime = Convert.ToDateTime("2020-1-1");
                    groupUesrDto.Role = UserRole.Member;
                    groupUesrDto.MessageRemindType = MessageRemindType.DISCARD;
                    groupUesrDto.CustomProperties = new Dictionary<string, string> { { "Number", Guid.NewGuid().ToString() } };
                }
                var updateGroupUserDataCommand = new UpdateGroupUserDataCommand { GroupUsers = groupUesrDtos, UserId = userId };
                await mediator.SendAsync<UpdateGroupUserDataCommand, SugarChatResponse>(updateGroupUserDataCommand);
                var groupUsersUpdateAfter = await repository.ToListAsync<GroupUser>();
                foreach (var groupUserUpdateAfter in groupUsersUpdateAfter)
                {
                    var groupUesrDto = groupUesrDtos.FirstOrDefault(x => x.Id == groupUserUpdateAfter.Id);
                    var groupUser = groupUsers.FirstOrDefault(x => x.Id == groupUserUpdateAfter.Id);
                    groupUserUpdateAfter.UserId.ShouldBe(groupUesrDto.UserId);
                    groupUserUpdateAfter.GroupId.ShouldBe(groupUesrDto.GroupId);
                    groupUserUpdateAfter.LastReadTime.ShouldBe(groupUesrDto.LastReadTime);
                    groupUserUpdateAfter.Role.ShouldBe(groupUesrDto.Role);
                    groupUserUpdateAfter.MessageRemindType.ShouldBe(groupUesrDto.MessageRemindType);
                    groupUserUpdateAfter.CreatedBy.ShouldBe(groupUser.CreatedBy);
                    groupUserUpdateAfter.CreatedDate.ShouldBe(groupUser.CreatedDate);
                }
                var groupUserIds = groupUsersUpdateAfter.Select(x => x.Id);
                (await repository.CountAsync<GroupUserCustomProperty>(x => groupUserIds.Contains(x.GroupUserId) && x.Key == "Number")).ShouldBe(3);
            }, builder =>
            {
                var iSecurityManager = Container.Resolve<ISecurityManager>();
                iSecurityManager.IsSupperAdmin().Returns(true);
                builder.RegisterInstance(iSecurityManager);
            });
        }

        [Fact]
        public async Task ShouldGetListByIds()
        {
            await Run<IMediator, IRepository, IGroupUserDataProvider>(async (mediator, repository, groupUserDataProvider) =>
            {
                var userIds = new string[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
                var groupId = Guid.NewGuid().ToString();
                for (int i = 0; i < userIds.Count(); i++)
                {
                    await repository.AddAsync(new User
                    {
                        Id = userIds[i]
                    });
                }
                var addGroupCommand = new AddGroupCommand
                {
                    Id = groupId,
                    UserId = userIds[0]
                };
                await mediator.SendAsync(addGroupCommand);
                var addGroupMemberCommand = new AddGroupMemberCommand
                {
                    AdminId = userIds[0],
                    GroupId = groupId,
                    GroupUserIds = userIds.Skip(1)
                };
                await mediator.SendAsync(addGroupMemberCommand);
                var getGroupMembersResponse = await mediator.RequestAsync<GetGroupMembersRequest, SugarChatResponse<IEnumerable<string>>>(new GetGroupMembersRequest
                {
                    GroupId = groupId
                });
                var groupUers = await groupUserDataProvider.GetMembersByGroupIdAsync(groupId);
                var resulut = await groupUserDataProvider.GetListByIdsAsync(groupUers.Select(x => x.Id));
                resulut.Count().ShouldBe(4);
            });
        }
    }
}