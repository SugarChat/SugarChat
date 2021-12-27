using Autofac;
using Mediator.Net;
using Microsoft.Extensions.Caching.Memory;
using Shouldly;
using SugarChat.Core;
using SugarChat.Core.Basic;
using SugarChat.Core.Cache;
using SugarChat.Core.Domain;
using SugarChat.Message.Exceptions;
using SugarChat.Core.IRepositories;
using SugarChat.Message;
using SugarChat.Message.Commands.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.IntegrationTest.Services
{
    public class InterfaceFixture : TestBase
    {
        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public async Task ShouldCreteUserWhenNeedUserExist(bool isExistCache, bool isExistDbData)
        {
            await Run<IMediator, IRepository, IMemoryCache>(async (mediator, repository, memoryCache) =>
            {
                var user = new User { Id = Guid.NewGuid().ToString() };
                var groupId = Guid.NewGuid().ToString();
                await repository.AddAsync(new Group { Id = groupId });
                await repository.AddAsync(new GroupUser { Id = Guid.NewGuid().ToString(), UserId = user.Id, GroupId = groupId });
                await repository.AddAsync(new Core.Domain.Message { Id = Guid.NewGuid().ToString(), GroupId = groupId });
                if (isExistCache)
                {
                    memoryCache.Set(CacheService.AllUser, new List<User> { user });
                }
                if (isExistDbData)
                {
                    await repository.AddAsync(user);
                }


                if (isExistDbData || (!isExistCache && !isExistDbData))
                {
                    var response = await mediator.SendAsync<SetMessageReadByUserBasedOnGroupIdCommand, SugarChatResponse>(
                        new SetMessageReadByUserBasedOnGroupIdCommand { UserId = user.Id, GroupId = groupId });
                    response.Code.ShouldBe(20000);
                    (await repository.FirstOrDefaultAsync<User>(x => x.Id == user.Id)).ShouldNotBeNull();
                }
                if (isExistDbData && !isExistDbData)
                {
                    var response = await mediator.SendAsync<SetMessageReadByUserBasedOnGroupIdCommand, SugarChatResponse>(
                        new SetMessageReadByUserBasedOnGroupIdCommand { UserId = user.Id, GroupId = groupId });
                    response.Message.ShouldBe(Prompt.UserNoExists.WithParams(user.Id).Message);
                }

                var usersForCache = memoryCache.Get<List<User>>(CacheService.AllUser);
                usersForCache.FirstOrDefault(x => x.Id == user.Id).ShouldNotBeNull();
            }, x =>
            {
                x.Register(x => new RunTimeProvider(RunTimeType.AspNetCoreApi)).InstancePerLifetimeScope();
            });
        }
    }
}
