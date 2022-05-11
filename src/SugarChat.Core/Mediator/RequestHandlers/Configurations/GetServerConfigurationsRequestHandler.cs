using System.Threading;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Configurations;
using SugarChat.Message.Basic;
using SugarChat.Message.Dtos.Configurations;
using SugarChat.Message.Requests.Configurations;

namespace SugarChat.Core.Mediator.RequestHandlers.Configurations
{
    public class GetServerConfigurationsRequestHandler : IRequestHandler<GetServerConfigurationsRequest, SugarChatResponse<ServerConfigurationsDto>>
    {
        private readonly IConfigurationService _configurationService;

        public GetServerConfigurationsRequestHandler(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task<SugarChatResponse<ServerConfigurationsDto>> Handle(IReceiveContext<GetServerConfigurationsRequest> context, CancellationToken cancellationToken)
        {
            var response = await _configurationService.GetServerConfigurationsAsync(context.Message, cancellationToken);
            return new SugarChatResponse<ServerConfigurationsDto>() { Data =  response.Configurations};
        }
    }
}
