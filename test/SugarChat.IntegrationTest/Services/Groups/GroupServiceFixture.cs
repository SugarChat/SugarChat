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
    public class GroupServiceFixture : TestFixtureBase
    {       
        [Fact]
        public async Task ShouldGetUserGroups()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {               
                var reponse = await mediator.RequestAsync<GetGroupsOfUserRequest, GetGroupsOfUserResponse>(new GetGroupsOfUserRequest { Id = userId });
                reponse.Groups.Count().ShouldBe(4);
            });
        }      

    }
}
