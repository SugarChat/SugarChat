using Autofac;
using Mediator.Net;
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
        private readonly IRepository _repository;
        private readonly IMediator _mediator;
        public UserServiceFixture()
        {
            _repository = Container.Resolve<IRepository>();
            _mediator = Container.Resolve<IMediator>();
        }

        [Fact]
        public async Task ShouldGetUserProfile()
        {
            await _repository.AddAsync(new User
            {
                Id = "b81cac07-1346-5417-318a-7a371b198511",
                CreatedBy = Guid.NewGuid().ToString(),
                CreatedDate = DateTimeOffset.Now,
                LastModifyBy = Guid.NewGuid().ToString(),
                CustomProperties = new Dictionary<string, string>(),
                LastModifyDate = DateTimeOffset.Now,
                DisplayName = "TestUser10",
                AvatarUrl = "",
            });

            await Task.Run(async () =>
            {
                var reponse = await _mediator.RequestAsync<GetUserRequest, GetUserResponse>(new GetUserRequest { Id = "b81cac07-1346-5417-318a-7a371b198511" });
                reponse.User.DisplayName.ShouldBe("TestUser10");
            });

            Dispose();
        }     

    }
}
