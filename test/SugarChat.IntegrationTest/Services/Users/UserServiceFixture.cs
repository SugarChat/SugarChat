﻿using Mediator.Net;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.IntegrationTest.Services.Users
{
    public class UserServiceFixture : TestBase
    {
        [Fact]
        public async Task ShouldGetUserProfile()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                var userId = Guid.NewGuid().ToString();
                await repository.AddAsync(new User
                {
                    Id = userId,
                    CreatedBy = Guid.NewGuid().ToString(),
                    CreatedDate = DateTimeOffset.Now,
                    LastModifyBy = Guid.NewGuid().ToString(),
                    CustomProperties = new Dictionary<string, string>(),
                    LastModifyDate = DateTimeOffset.Now,
                    DisplayName = "TestUser10",
                    AvatarUrl = "",
                });
                var reponse = await mediator.RequestAsync<GetUserRequest, GetUserResponse>(new GetUserRequest { Id = userId });
                reponse.User.DisplayName.ShouldBe("TestUser10");
            });
        }

    }
}
