﻿using Mediator.Net;
using Shouldly;
using SugarChat.Core.Basic;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Requests;
using SugarChat.Message.Requests.Groups;
using SugarChat.Message.Responses;
using SugarChat.Message.Responses.Groups;
using SugarChat.Shared.Dtos;
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
            await Run<IMediator>(async (mediator) =>
            {
                var reponse = await mediator.RequestAsync<GetGroupsOfUserRequest, SugarChatResponse<IEnumerable<GroupDto>>>(new GetGroupsOfUserRequest { Id = userId });
                reponse.Data.Count().ShouldBe(4);
            });
        }

        [Fact]
        public async Task ShouldGetGroupProfile()
        {
            await Run<IMediator>(async (mediator) =>
            {
                var reponse = await mediator.RequestAsync<GetGroupProfileRequest, SugarChatResponse<GroupDto>>(new GetGroupProfileRequest { UserId = userId, GroupId = conversationId });
                reponse.Data.Name.ShouldBe("TestGroup3");
                reponse.Data.MemberCount.ShouldBe(2);
            });
        }

        [Fact]
        public async Task ShouldUpdateGroupProfile()
        {
            await Run<IMediator>(async (mediator) =>
            {
                await mediator.SendAsync<UpdateGroupProfileCommand, SugarChatResponse> (new UpdateGroupProfileCommand
                {
                    Id = conversationId,
                    Name = "内部沟通群",
                    Description = "进行及时有效的沟通"

                }, default(CancellationToken));

                var reponse = await mediator.RequestAsync<GetGroupProfileRequest, SugarChatResponse<GroupDto>>(new GetGroupProfileRequest { UserId = userId, GroupId = conversationId });
                reponse.Data.Name.ShouldBe("内部沟通群");                
            });
        }

    }
}
