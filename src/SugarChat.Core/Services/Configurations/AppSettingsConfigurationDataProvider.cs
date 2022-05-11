using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SugarChat.Core.Services.Configurations
{
    public class AppSettingsConfigurationDataProvider : IConfigurationDataProvider
    {
        private readonly IConfiguration _configuration;

        public AppSettingsConfigurationDataProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<ServerConfigurations> GetServerConfigurationsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_configuration.GetSection("ServerConfigurations").Get<ServerConfigurations>());
        }
    }
}