using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Commands.Emotions;
using SugarChat.Message.Requests.Emotions;
using SugarChat.Message.Responses.Emotions;

namespace SugarChat.Core.Services.Emotions;

public interface IEmotionService : IService
{
    Task<GetEmotionByIdsResponse> GetEmotionByIdsAsync(GetEmotionByIdsRequest request,
        CancellationToken cancellationToken = default);

    Task<GetUserEmotionsResponse> GetUserEmotionsAsync(GetUserEmotionsRequest request,
        CancellationToken cancellationToken = default);

    Task AddEmotionAsync(AddEmotionCommand command, CancellationToken cancellationToken = default);
    Task RemoveEmotionAsync(RemoveEmotionCommand command, CancellationToken cancellationToken = default);
}