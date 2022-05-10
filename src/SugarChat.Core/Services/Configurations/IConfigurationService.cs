using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Requests.Configurations;
using SugarChat.Message.Responses.Configurations;

namespace SugarChat.Core.Services.Configurations
{
    public interface IConfigurationService : IService
    {
        Task<GetServerConfigurationsResponse> GetServerConfigurationsAsync(GetServerConfigurationsRequest request,
            CancellationToken cancellationToken = default);
    }
}