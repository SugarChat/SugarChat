using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Configurations
{
    public interface IConfigurationDataProvider : IDataProvider
    {
        Task<ServerConfigurations> GetServerConfigurationsAsync(CancellationToken cancellationToken = default);
    }
}