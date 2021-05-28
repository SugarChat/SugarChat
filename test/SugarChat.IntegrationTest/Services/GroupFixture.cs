using Mediator.Net;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Mediator.CommandHandlers.Groups;
using SugarChat.Core.Services;
using SugarChat.Message.Commands.Groups;
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
                    Id = Guid.NewGuid().ToString()
                };
                await mediator.SendAsync<AddGroupCommand, AddGroupResponse>(command);

                (await repository.AnyAsync<Group>(x => x.Id == command.Id)).ShouldBe(true);
            });
        }

        [Fact]
        public async Task ShouldDismissGroup()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                string groupId = Guid.NewGuid().ToString();
                await repository.AddAsync(new Group
                {
                    Id = groupId
                });

                DismissGroupCommand command = new DismissGroupCommand
                {
                    GroupId = Guid.NewGuid().ToString()
                };
                Func<Task> funcTask = () => mediator.SendAsync(command);
                funcTask.ShouldThrow(typeof(BusinessWarningException)).Message.ShouldBe(string.Format(ServiceCheckExtensions.GroupNoExists, command.GroupId));

                command.GroupId = groupId;
                await mediator.SendAsync(command);
                (await repository.AnyAsync<Group>(x => x.Id == command.GroupId && x.IsDel)).ShouldBe(true);
            });
        }
    }
}