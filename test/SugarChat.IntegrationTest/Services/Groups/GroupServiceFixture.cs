using Autofac;
using Mediator.Net;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.IntegrationTest.Services.Groups
{
    public class GroupServiceFixture : TestBase
    {
        private readonly IRepository _repository;
        private readonly IMediator _mediator;
        public GroupServiceFixture()
        {
            _repository = Container.Resolve<IRepository>();
            _mediator = Container.Resolve<IMediator>();
        }

        private List<User> users = new List<User>();
        private List<Group> groups = new List<Group>();
        private List<GroupUser> groupUsers = new List<GroupUser>();
        private List<Friend> friends = new List<Friend>();
        private List<Core.Domain.Message> messages = new List<Core.Domain.Message>();

        [Fact]
        public async Task ShouldGetUserGroups()
        {
            await GenerateTestCollections();

            await Task.Run(async () =>
            {
                var reponse = await _mediator.RequestAsync<GetGroupsOfUserRequest, GetGroupsOfUserResponse>(new GetGroupsOfUserRequest { Id = "b81cac07-1346-5417-318a-7a371b198511" });
                reponse.Groups.Count().ShouldBe(4);
            });

            Dispose();
        }

        private async Task GenerateTestCollections()
        {
            groups = new List<Group>
            {
               new Group
               {
                    AvatarUrl = "",
                    CreatedBy = Guid.NewGuid().ToString(),
                    CreatedDate = DateTime.UtcNow,
                    CustomProperties = new Dictionary<string, string>(),
                    Description = "A Test Group!",
                    Id = "25284ca8-74f6-6774-9e42-2b8744fa5e63",
                    LastModifyDate = DateTime.UtcNow,
                    LastModifyBy = Guid.NewGuid().ToString(),
                    Name = "TestGroup1"
               },
               new Group
               {
                    AvatarUrl = "",
                    CreatedBy = Guid.NewGuid().ToString(),
                    CreatedDate = DateTime.UtcNow,
                    CustomProperties = new Dictionary<string, string>(),
                    Description = "A Test Group!",
                    Id ="48b0e264-f874-fa9c-3503-082539733eab",
                    LastModifyDate = DateTime.UtcNow,
                    LastModifyBy = Guid.NewGuid().ToString(),
                    Name = "TestGroup2"
               },
               new Group
               {
                    AvatarUrl = "",
                    CreatedBy = Guid.NewGuid().ToString(),
                    CreatedDate = DateTime.UtcNow,
                    CustomProperties = new Dictionary<string, string>(),
                    Description = "A Test Group!",
                    Id = "03c7049f-0455-344f-d291-771530ff436b",
                    LastModifyDate = DateTime.UtcNow,
                    LastModifyBy = Guid.NewGuid().ToString(),
                    Name = "TestGroup3"
               },
               new Group
               {
                    AvatarUrl = "",
                    CreatedBy = Guid.NewGuid().ToString(),
                    CreatedDate = DateTime.UtcNow,
                    CustomProperties = new Dictionary<string, string>(),
                    Description = "A Test Group!",
                    Id = "dfbce77c-d12c-8f1f-50bd-5dbe93ab14f0",
                    LastModifyDate = DateTime.UtcNow,
                    LastModifyBy = Guid.NewGuid().ToString(),
                    Name = "TestGroup4"
               },
               new Group
               {
                    AvatarUrl = "",
                    CreatedBy = Guid.NewGuid().ToString(),
                    CreatedDate = DateTime.UtcNow,
                    CustomProperties = new Dictionary<string, string>(),
                    Description = "A Test Group!",
                    Id = "807b12e5-fc4a-6847-844d-050d1d1a27e2",
                    LastModifyDate = DateTime.UtcNow,
                    LastModifyBy = Guid.NewGuid().ToString(),
                    Name = "TestGroup5"
               },
               new Group
               {
                    AvatarUrl = "",
                    CreatedBy = Guid.NewGuid().ToString(),
                    CreatedDate = DateTime.UtcNow,
                    CustomProperties = new Dictionary<string, string>(),
                    Description = "A Test Group!",
                    Id = "e4176346-85b6-f5de-241b-5ffe62e871ff",
                    LastModifyDate = DateTime.UtcNow,
                    LastModifyBy = Guid.NewGuid().ToString(),
                    Name = "TestGroup6"
               }
            };

            users = new List<User>
            {
                new User
                {
                   Id="719ae9fe-4099-90a3-2201-06a5f183ef57",
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   DisplayName="TestUser1",
                   AvatarUrl="",
                },
                new User
                {
                   Id="6fbc001b-0e95-45ac-44b5-df601c5eba3a",
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   DisplayName="TestUser2",
                   AvatarUrl="",
                },
                new User
                {
                   Id="5c8f70ef-b4f4-7dfb-ddde-142ee2a551f4",
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   DisplayName="TestUser3",
                   AvatarUrl="",
                },
                new User
                {
                   Id="1cea575f-4882-b8e9-c644-e032117ff999",
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   DisplayName="TestUser4",
                   AvatarUrl="",
                },
                new User
                {
                   Id="cbb40df7-e7cf-2725-d4dd-628cc53e57fc",
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   DisplayName="TestUser5",
                   AvatarUrl="",
                },
                new User
                {
                   Id="6348e78f-e92b-0b6e-9196-8d53b4cda90f",
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   DisplayName="TestUser6",
                   AvatarUrl="",
                },
                new User
                {
                   Id="a83240d3-b2f9-4062-c959-6926dc7fff98",
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   DisplayName="TestUser7",
                   AvatarUrl="",
                },
                new User
                {
                   Id="235e60be-dc2a-e868-696e-2329052d7bb7",
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   DisplayName="TestUser8",
                   AvatarUrl="",
                },
                new User
                {
                   Id="38784f63-b2f2-ae70-c17e-3331270a5a67",
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   DisplayName="TestUser9",
                   AvatarUrl="",
                },
                new User
                {
                   Id="b81cac07-1346-5417-318a-7a371b198511",
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   DisplayName="TestUser10",
                   AvatarUrl="",
                }
            };

            friends = new List<Friend>
            {
                new Friend
                {
                   Id="bc8ba9e0-412b-7ae6-bf38-2a0ddccb237d",
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId="b81cac07-1346-5417-318a-7a371b198511", //用户10
                   FriendId="38784f63-b2f2-ae70-c17e-3331270a5a67", //用户9
                   BecomeFriendAt=DateTimeOffset.Now
                }

            };

            groupUsers = new List<GroupUser>
            {
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId="b81cac07-1346-5417-318a-7a371b198511", //用户10
                   GroupId="25284ca8-74f6-6774-9e42-2b8744fa5e63",//组1
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId="38784f63-b2f2-ae70-c17e-3331270a5a67", //用户9
                   GroupId="25284ca8-74f6-6774-9e42-2b8744fa5e63",//组1
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId="235e60be-dc2a-e868-696e-2329052d7bb7", //用户8
                   GroupId="25284ca8-74f6-6774-9e42-2b8744fa5e63",//组1
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId="b81cac07-1346-5417-318a-7a371b198511", //用户10
                   GroupId="48b0e264-f874-fa9c-3503-082539733eab",//组2
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId="cbb40df7-e7cf-2725-d4dd-628cc53e57fc", //用户5
                   GroupId="48b0e264-f874-fa9c-3503-082539733eab",//组2
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId="6348e78f-e92b-0b6e-9196-8d53b4cda90f", //用户6
                   GroupId="48b0e264-f874-fa9c-3503-082539733eab",//组2
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId="a83240d3-b2f9-4062-c959-6926dc7fff98", //用户7
                   GroupId="48b0e264-f874-fa9c-3503-082539733eab",//组2
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId="b81cac07-1346-5417-318a-7a371b198511", //用户10
                   GroupId="03c7049f-0455-344f-d291-771530ff436b",//组3
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId="38784f63-b2f2-ae70-c17e-3331270a5a67", //用户9
                   GroupId="03c7049f-0455-344f-d291-771530ff436b",//组3
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId="719ae9fe-4099-90a3-2201-06a5f183ef57", //用户1
                   GroupId="dfbce77c-d12c-8f1f-50bd-5dbe93ab14f0",//组4
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId="b81cac07-1346-5417-318a-7a371b198511", //用户10
                   GroupId="dfbce77c-d12c-8f1f-50bd-5dbe93ab14f0",//组4
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId="6fbc001b-0e95-45ac-44b5-df601c5eba3a", //用户2
                   GroupId="dfbce77c-d12c-8f1f-50bd-5dbe93ab14f0",//组4
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId="1cea575f-4882-b8e9-c644-e032117ff999", //用户4                                                                                                                                                                                                       
                   GroupId="dfbce77c-d12c-8f1f-50bd-5dbe93ab14f0",//组4
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId="5c8f70ef-b4f4-7dfb-ddde-142ee2a551f4", //用户3
                   GroupId="dfbce77c-d12c-8f1f-50bd-5dbe93ab14f0",//组4
                },
            };

            messages = new List<Core.Domain.Message>
            {
               new Core.Domain.Message
               {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   GroupId="03c7049f-0455-344f-d291-771530ff436b",
                   Content="我通过了你的朋友验证请求,现在我们可以开始聊天了",
                   ParsedContent="我通过了你的朋友验证请求,现在我们可以开始聊天了",
                   Type=MessageType.Text,
                   SubType=0,
                   SentBy="38784f63-b2f2-ae70-c17e-3331270a5a67",    //用户9通过用户10的好友发送的一条消息
                   SentTime=DateTimeOffset.Now,
                   IsDel=false,
                   IsSystem=false
               },
               new Core.Domain.Message
               {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   GroupId="03c7049f-0455-344f-d291-771530ff436b",
                   Content="你好",
                   ParsedContent="你好",
                   Type=MessageType.Text,
                   SubType=0,
                   SentBy="38784f63-b2f2-ae70-c17e-3331270a5a67",    //用户9
                   SentTime=DateTimeOffset.Now,
                   IsDel=false,
                   IsSystem=false
               },
               new Core.Domain.Message
               {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   GroupId="03c7049f-0455-344f-d291-771530ff436b",
                   Content="[图片]",
                   ParsedContent="图片转化后的内容",
                   Type=MessageType.Image,
                   SubType=0,
                   SentBy="b81cac07-1346-5417-318a-7a371b198511",    //用户10
                   SentTime=DateTimeOffset.Now,
                   IsDel=false,
                   IsSystem=false
               },

               new Core.Domain.Message
               {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   GroupId="dfbce77c-d12c-8f1f-50bd-5dbe93ab14f0",
                   Content="TestUser1邀请TestUser2加入了群聊",
                   ParsedContent="TestUser1邀请TestUser2加入了群聊",
                   Type=MessageType.Text,
                   SubType=0,
                   SentBy="",
                   SentTime=DateTimeOffset.Now,
                   IsDel=false,
                   IsSystem=true
               },
               new Core.Domain.Message
               {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   GroupId="dfbce77c-d12c-8f1f-50bd-5dbe93ab14f0",
                   Content="今天真热啊",
                   ParsedContent="今天真热啊",
                   Type=MessageType.Text,
                   SubType=0,
                   SentBy="719ae9fe-4099-90a3-2201-06a5f183ef57", //用户1
                   SentTime=DateTimeOffset.Now,
                   IsDel=false,
                   IsSystem=true
               },
               new Core.Domain.Message
               {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   GroupId="dfbce77c-d12c-8f1f-50bd-5dbe93ab14f0",
                   Content="是啊,又到了不动都能出汗的季节了",
                   ParsedContent="是啊,又到了不动都能出汗的季节了",
                   Type=MessageType.Text,
                   SubType=0,
                   SentBy="6fbc001b-0e95-45ac-44b5-df601c5eba3a", //用户2
                   SentTime=DateTimeOffset.Now,
                   IsDel=false,
                   IsSystem=true
               },
               new Core.Domain.Message
               {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   GroupId="dfbce77c-d12c-8f1f-50bd-5dbe93ab14f0",
                   Content="谁说不是呢",
                   ParsedContent="谁说不是呢",
                   Type=MessageType.Text,
                   SubType=0,
                   SentBy="5c8f70ef-b4f4-7dfb-ddde-142ee2a551f4", //用户3
                   SentTime=DateTimeOffset.Now,
                   IsDel=false,
                   IsSystem=true
               }
            };

            await _repository.AddRangeAsync(users, default(CancellationToken));
            await _repository.AddRangeAsync(groups, default(CancellationToken));
            await _repository.AddRangeAsync(friends, default(CancellationToken));
            await _repository.AddRangeAsync(groupUsers, default(CancellationToken));
            await _repository.AddRangeAsync(messages, default(CancellationToken));
        }

    }
}
