using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message;
using SugarChat.Message.Commands.Friends;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Requests;
using SugarChat.Message.Requests.Groups;
using SugarChat.Net.Client;
using SugarChat.Shared.Dtos;
using SugarChat.Shared.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.IntegrationTest
{
    public class SugarChatClientFixture : TestBase
    {

        private string[] _userIds = new[] { Guid.NewGuid().ToString(),Guid.NewGuid().ToString(),Guid.NewGuid().ToString(),
                                           Guid.NewGuid().ToString(),Guid.NewGuid().ToString(),Guid.NewGuid().ToString() };

        private string[] _groupIds = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };

        public SugarChatClientFixture()
        {
            GenerateTestData().Wait();
        }

        private async Task GenerateTestData()
        {
            await CreateUsersAsync().ConfigureAwait(false);
            await CreateGroupsAsync().ConfigureAwait(false);
            await AddFriendsAsync().ConfigureAwait(false);
            await AddGroupUsersAsync().ConfigureAwait(false);
        }

        private async Task CreateUsersAsync()
        {
            await Run<ISugarChatClient>(async (client) =>
            {
                for (var i = 0; i < _userIds.Length; i++)
                {
                    await client.CreateUserAsync(new AddUserCommand
                    {
                        Id = _userIds[i],
                        DisplayName = $"TestUser{i}"
                    });
                }
            });
        }

        private async Task CreateGroupsAsync()
        {
            await Run<ISugarChatClient>(async (client) =>
            {
                for (var i = 0; i < _groupIds.Length; i++)
                {
                    await client.CreateGroupAsync(new AddGroupCommand
                    {
                        Id = _groupIds[i],
                        Name = $"TestGroup{i}",
                        Description = $"测试添加组{i}"
                    });
                }
            });

            await Run<IRepository>(async (repository) =>
            {
                var groups = await repository.ToListAsync<Group>().ConfigureAwait(false);
            });
        }

        private async Task AddFriendsAsync()
        {
            await Run<ISugarChatClient>(async (client) =>
            {
                await client.AddFriendAsync(new AddFriendCommand
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = _userIds[0],
                    FriendId = _userIds[1],
                    BecomeFriendAt = DateTimeOffset.Now
                });
            });
        }

        private async Task AddGroupUsersAsync()
        {
            await Run<IRepository>(async (repository) =>
            {
                await repository.AddRangeAsync(new List<GroupUser>
                {
                   new GroupUser
                   {
                        Id = Guid.NewGuid().ToString(),
                        UserId = _userIds[0],
                        GroupId = _groupIds[0],
                        Role = UserRole.Admin
                   },
                   new GroupUser
                   {
                        Id = Guid.NewGuid().ToString(),
                        UserId = _userIds[0],
                        GroupId = _groupIds[2],
                        Role = UserRole.Admin
                   }
                });
            });
        }

        [Fact]
        public async Task MockCreateUser()
        {
            await Run<ISugarChatClient>(async (client) =>
            {
                var userId = Guid.NewGuid().ToString();
                await client.CreateUserAsync(new AddUserCommand
                {
                    Id = userId,
                    DisplayName = "TestUser"
                });

                var result = await client.GetUserProfileAsync(new GetUserRequest { Id = userId });
                result.Data.DisplayName.ShouldBe("TestUser");
            });
        }

        [Fact]
        public async Task MockUpdateMyProfile()
        {
            await Run<ISugarChatClient>(async (client) =>
            {
                await client.CreateUserAsync(new AddUserCommand
                {
                    Id = _userIds[0],
                    DisplayName = "TestUser"
                });

                await client.UpdateMyProfileAsync(new UpdateUserCommand
                {
                    Id = _userIds[0],
                    DisplayName = "Update"
                });

                var result = await client.GetUserProfileAsync(new GetUserRequest { Id = _userIds[0] });
                result.Data.DisplayName.ShouldBe("Update");
            });
        }

        [Fact]
        public async Task MockCreateGroup()
        {
            await Run<ISugarChatClient, IRepository>(async (client, repository) =>
            {
                var groupId = Guid.NewGuid().ToString();
                await client.CreateGroupAsync(new AddGroupCommand
                {
                    Id = groupId,
                    Name = "TestGroup",
                    Description = "测试添加组"
                });

                var group = await repository.FirstOrDefaultAsync<Group>(x => x.Id == groupId).ConfigureAwait(false);
                group.Name.ShouldBe("TestGroup");
            });
        }

        [Fact]
        public async Task MockAddGroupMember()
        {
            await Run<ISugarChatClient, IRepository>(async (client, repository) =>
            {
                await client.AddGroupMemberAsync(new AddGroupMemberCommand
                {
                    GroupId = _groupIds[0],
                    AdminId = _userIds[0],
                    GroupUsers = new List<AddGroupUserDto>
                    {
                        new AddGroupUserDto
                        {
                            Id = Guid.NewGuid().ToString(),
                            GroupId = _groupIds[0],
                            UserId = _userIds[1]
                        },
                        new AddGroupUserDto
                        {
                            Id = Guid.NewGuid().ToString(),
                            GroupId = _groupIds[0],
                            UserId = _userIds[2]
                        }
                    }
                });

                var groupUsers = await repository.CountAsync<GroupUser>(x => x.GroupId == _groupIds[0]).ConfigureAwait(false);
                groupUsers.ShouldBe(3);
            });
        }

        [Fact]
        public async Task MockDismissGroup()
        {
            await Run<ISugarChatClient, IRepository>(async (client, repository) =>
             {    
                 await client.AddGroupMemberAsync(new AddGroupMemberCommand
                 {
                     GroupId = _groupIds[2],
                     AdminId = _userIds[0],
                     GroupUsers = new List<AddGroupUserDto>
                    {
                        new AddGroupUserDto
                        {
                            Id = Guid.NewGuid().ToString(),
                            GroupId = _groupIds[2],
                            UserId = _userIds[3]
                        },
                        new AddGroupUserDto
                        {
                            Id = Guid.NewGuid().ToString(),
                            GroupId = _groupIds[2],
                            UserId = _userIds[4]
                        },
                        new AddGroupUserDto
                        {
                            Id = Guid.NewGuid().ToString(),
                            GroupId = _groupIds[2],
                            UserId = _userIds[5]
                        }
                    }
                 });

                 var result = await client.GetGroupMemberListAsync(new GetMembersOfGroupRequest
                 {
                     UserId = _userIds[3],
                     GroupId = _groupIds[2]
                 });

                 result.Data.Count().ShouldBe(4);

                 await client.DismissGroupAsync(new DismissGroupCommand
                 {
                     GroupId = _groupIds[2]
                 });

                 (await repository.ToListAsync<GroupUser>(x => x.GroupId == _groupIds[2]).ConfigureAwait(false)).Count.ShouldBe(0);
             });
        }

        [Fact]
        public async Task MockGetGroupList()
        {
            await Run<ISugarChatClient>(async (client) =>
            {                
                var result = await client.GetGroupListAsync(new GetGroupsOfUserRequest
                {
                    Id = _userIds[0],
                    PageSettings = new PageSettings
                    {
                        PageNum = 1,
                        PageSize = 10
                    }
                });

                result.Data.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task MockGetGroupProfile()
        {    
            await Run<ISugarChatClient>(async (client) =>
            {              
                var result = await client.GetGroupProfileAsync(new GetGroupProfileRequest
                {
                    UserId = _userIds[0],
                    GroupId = _groupIds[0]
                });
                result.Data.Name.ShouldBe("TestGroup");
            });
        }

        [Fact]
        public async Task MockUpdateGroupProfile()
        {
            await Run<ISugarChatClient>(async (client) =>
            {              
                await client.UpdateGroupProfileAsync(new UpdateGroupProfileCommand
                {
                    Id = _groupIds[0],
                    Description = "TestUpdateGroup"
                });

                var result = await client.GetGroupProfileAsync(new GetGroupProfileRequest
                {
                    UserId = _userIds[0],
                    GroupId = _groupIds[0]
                });
                result.Data.Description.ShouldBe("TestUpdateGroup");
            });
        }

        [Fact]
        public async Task MockRemoveGroup()
        {
            await Run<ISugarChatClient, IRepository>(async (client, repository) =>
             {                
                 var groupId = Guid.NewGuid().ToString();
                 await client.CreateGroupAsync(new AddGroupCommand
                 {
                     Id = groupId
                 });

                 await client.RemoveGroupAsync(new RemoveGroupCommand
                 {
                     Id = groupId
                 });

                 var group = await repository.FirstOrDefaultAsync<Group>(x => x.Id == groupId).ConfigureAwait(false);
                 group.ShouldBeNull();
             });
        }

        [Fact]
        public async Task MockJoinGroup()
        {
            await Run<ISugarChatClient, IRepository>(async (client, repository) =>
             {                
                 await client.JoinGroupAsync(new JoinGroupCommand
                 {
                     Id = Guid.NewGuid().ToString(),
                     GroupId = _groupIds[3],
                     UserId = _userIds[3],
                 });

                 (await repository.FirstOrDefaultAsync<GroupUser>(x => x.GroupId == _groupIds[3]).ConfigureAwait(false)).ShouldNotBeNull();

                 await client.QuitGroupAsync(new QuitGroupCommand
                 {
                     GroupId = _groupIds[3],
                     UserId = _userIds[3]
                 });

                 (await repository.FirstOrDefaultAsync<GroupUser>(x => x.GroupId == _groupIds[3] && x.UserId == _userIds[3]).ConfigureAwait(false)).ShouldBeNull();
             });
        }

        [Fact]
        public async Task MockChangeGroupOwner()
        {
            await Run<ISugarChatClient, IRepository>(async (client, repository) =>
            {              
                await repository.AddRangeAsync(new List<GroupUser>
                {
                   new GroupUser
                   {
                        Id = Guid.NewGuid().ToString(),
                        UserId = _userIds[0],
                        GroupId = _groupIds[1],
                        Role = UserRole.Owner
                   },
                   new GroupUser
                   {
                        Id = Guid.NewGuid().ToString(),
                        UserId = _userIds[1],
                        GroupId = _groupIds[1],
                        Role = UserRole.Member
                   },
                   new GroupUser
                   {
                        Id = Guid.NewGuid().ToString(),
                        UserId = _userIds[2],
                        GroupId = _groupIds[1],
                        Role = UserRole.Member
                   }
                });

                await client.ChangeGroupOwnerAsync(new ChangeGroupOwnerCommand
                {
                    OwnerId = _userIds[0],
                    GroupId = _groupIds[1],
                    NewOwnerId = _userIds[1]
                });

                var result = await repository.FirstOrDefaultAsync<GroupUser>(x => x.UserId == _userIds[1] && x.GroupId == _groupIds[1] && x.Role == UserRole.Owner);
                result.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task MockDeleteGroupMember()
        {
            await Run<ISugarChatClient, IRepository>(async (client, repository) =>
            {   
                await repository.AddRangeAsync(new List<GroupUser>
                {
                   new GroupUser
                   {
                        Id = Guid.NewGuid().ToString(),
                        UserId = _userIds[0],
                        GroupId = _groupIds[1],
                        Role = UserRole.Admin
                   },
                   new GroupUser
                   {
                        Id = Guid.NewGuid().ToString(),
                        UserId = _userIds[1],
                        GroupId = _groupIds[1],
                        Role = UserRole.Member
                   },
                   new GroupUser
                   {
                        Id = Guid.NewGuid().ToString(),
                        UserId = _userIds[2],
                        GroupId = _groupIds[1],
                        Role = UserRole.Member
                   }
                }).ConfigureAwait(false);

                await client.DeleteGroupMemberAsync(new RemoveGroupMemberCommand
                {
                    AdminId = _userIds[0],
                    GroupId = _groupIds[1],
                    UserIdList = new List<string> { _userIds[2] }
                });

                var result = await repository.FirstOrDefaultAsync<GroupUser>(x => x.GroupId == _groupIds[0] && x.UserId == _userIds[2]).ConfigureAwait(false);
                result.ShouldBeNull();

            });
        }

        [Fact]
        public async Task MockSetMessageRemindType()
        {
            await Run<ISugarChatClient, IRepository>(async (client, repository) =>
            {
                await client.SetMessageRemindTypeAsync(new SetMessageRemindTypeCommand
                {
                    UserId = _userIds[0],
                    GroupId = _groupIds[0],
                    MessageRemindType = MessageRemindType.ACPT_AND_NOTE
                });

                var result = await repository.FirstOrDefaultAsync<GroupUser>(x => x.UserId == _userIds[0] && x.GroupId == _groupIds[0]).ConfigureAwait(false);
                result.MessageRemindType.ShouldBe(MessageRemindType.ACPT_AND_NOTE);
            });
        }

        [Fact]
        public async Task MockSetGroupMemberRole()
        {
            await Run<ISugarChatClient, IRepository>(async (client, repository) =>
            {
                await repository.AddRangeAsync(new List<GroupUser>
                {
                   new GroupUser
                   {
                        Id = Guid.NewGuid().ToString(),
                        UserId = _userIds[1],
                        GroupId = _groupIds[0],
                        Role = UserRole.Owner
                   },
                   new GroupUser
                   {
                        Id = Guid.NewGuid().ToString(),
                        UserId = _userIds[2],
                        GroupId = _groupIds[0],
                        Role = UserRole.Member
                   }
                });

                await client.SetGroupMemberRoleAsync(new SetGroupMemberRoleCommand
                {
                    OwnerId = _userIds[1],
                    GroupId = _groupIds[0],
                    MemberId = _userIds[2],
                    Role = UserRole.Admin
                });

                var result = await repository.ToListAsync<GroupUser>(x => x.GroupId == _groupIds[0]).ConfigureAwait(false);
                result.Where(x => x.Role == UserRole.Admin).Count().ShouldBe(2);
            });
        }

        [Fact]
        public async Task MockAddFriend()
        {   
            await Run<ISugarChatClient, IRepository>(async (client, repository) =>
             {
                 await client.AddFriendAsync(new AddFriendCommand
                 {
                     Id = Guid.NewGuid().ToString(),
                     UserId = _userIds[0],
                     FriendId = _userIds[2],
                     BecomeFriendAt = DateTimeOffset.Now
                 });

                 var userFriend = await repository.FirstOrDefaultAsync<Friend>(x => x.UserId == _userIds[0] && x.FriendId == _userIds[2]).ConfigureAwait(false);
                 userFriend.ShouldNotBeNull();
             });
        }

        [Fact]
        public async Task MockRemoveFriend()
        {
            await Run<ISugarChatClient, IRepository>(async (client, repository) =>
            {     
                await client.RemoveFriendAsync(new RemoveFriendCommand
                {
                    UserId = _userIds[0],
                    FriendId = _userIds[1]
                });
                var friend = await repository.FirstOrDefaultAsync<Friend>(x => x.UserId == _userIds[0] && x.FriendId == _userIds[1]);
                friend.ShouldBeNull();
            });
        }

    }
}
