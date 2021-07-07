using Mediator.Net;
using Shouldly;
using SugarChat.Core.Basic;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Mediator.CommandHandlers.Groups;
using SugarChat.Core.Services;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Requests.Groups;
using SugarChat.Message.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

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
                    CustomProperties=new Dictionary<string, string> { { "MerchId","1"}, { "OrderId", "2" } }
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
                var group = await repository.SingleAsync<Group>(x => x.Id == command.Id);
                group.CustomProperties.GetValueOrDefault("MerchId").ShouldBe("1");
                group.CustomProperties.GetValueOrDefault("OrderId").ShouldBe("2");
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
                    Id = Guid.NewGuid().ToString()
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
                (await repository.CountAsync<Group>()).ShouldBe(4);
                (await repository.CountAsync<GroupUser>()).ShouldBe(8);
                (await repository.CountAsync<Core.Domain.Message>()).ShouldBe(12);
            });
        }

        [Fact]
        public async Task ShouldGetByCustomProperties()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                string userId = Guid.NewGuid().ToString();
                for (int i = 0; i < 3; i++)
                {
                    string groupId = Guid.NewGuid().ToString();
                    await repository.AddAsync(new GroupUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = userId,
                        GroupId = groupId
                    });
                    await repository.AddAsync(new Group
                    {
                        Id = groupId,
                        CustomProperties = new Dictionary<string, string> { { "merchId", $"{i + 1}{i + 1}" }, { "userId", $"{i + 1}{i + 2}" } }
                    });
                }
                {
                    var response = await mediator.RequestAsync<GetGroupByCustomPropertiesRequest, SugarChatResponse<IEnumerable<GroupDto>>>(new GetGroupByCustomPropertiesRequest()
                    {
                        UserId = Guid.NewGuid().ToString()
                    });
                    response.Message.ShouldBe(Prompt.ParameterRequired.WithParams("groupIds").Message);
                }
                {
                    var response = await mediator.RequestAsync<GetGroupByCustomPropertiesRequest, SugarChatResponse<IEnumerable<GroupDto>>>(new GetGroupByCustomPropertiesRequest()
                    {
                        UserId = userId,
                        CustomPropertys = new Dictionary<string, string> { { "merchId", "11" }, { "userId", "12" } }
                    });
                    response.Data.Count().ShouldBe(1);
                }
            });
        }
    }
}