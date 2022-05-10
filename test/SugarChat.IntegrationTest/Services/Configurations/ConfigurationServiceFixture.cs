using System;
using System.Threading.Tasks;
using Mediator.Net;
using Shouldly;
using SugarChat.Message.Basic;
using SugarChat.Message.Dtos.Configurations;
using SugarChat.Message.Requests.Configurations;
using Xunit;

namespace SugarChat.IntegrationTest.Services.Configurations
{
    public class ConfigurationServiceFixture : TestFixtureBase
    {
       [Fact]
        public async Task ShouldGetServerConfigurations()
        {
            
            await Run<IMediator>(async (configurationService) =>
            {
                var revokeLimitTimeSpan = TimeSpan.Parse(_configuration["ServerConfigurations:RevokeLimitTimeSpan"]);
                var response = await configurationService.RequestAsync<GetServerConfigurationsRequest, SugarChatResponse<ServerConfigurationsDto>>(new GetServerConfigurationsRequest());
                response.Data.RevokeLimitTimeSpan.ShouldBe(revokeLimitTimeSpan);
            });
      
        }
    }
}
