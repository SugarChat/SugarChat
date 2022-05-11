using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SugarChat.Message.Dtos.Configurations;
using SugarChat.Message.Requests.Configurations;
using SugarChat.Message.Responses.Configurations;

namespace SugarChat.Core.Services.Configurations
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IMapper _mapper;
        private readonly IConfigurationDataProvider _configurationDataProvider;


        public ConfigurationService(
            IMapper mapper, IConfigurationDataProvider configurationDataProvider)
        {
            _mapper = mapper;
            _configurationDataProvider = configurationDataProvider;
        }


        public async Task<GetServerConfigurationsResponse> GetServerConfigurationsAsync(
            GetServerConfigurationsRequest request,
            CancellationToken cancellationToken = default)
        {
            return new()
            {
                Configurations = _mapper.Map<ServerConfigurationsDto>(await _configurationDataProvider
                    .GetServerConfigurationsAsync(cancellationToken).ConfigureAwait(false))
            };
        }
    }
}