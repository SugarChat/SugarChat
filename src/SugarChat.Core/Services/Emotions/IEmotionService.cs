using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Commands.Emotions;
using SugarChat.Message.Dtos.Emotions;
using SugarChat.Message.Requests.Emotions;

namespace SugarChat.Core.Services.Emotions;

public interface IEmotionService : IService
{
    Task<IEnumerable<EmotionDto>> GetEmotionByIdsAsync(GetEmotionByIdsRequest request,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<EmotionDto>> GetUserEmotionsAsync(GetUserEmotionsRequest request,
        CancellationToken cancellationToken = default);

    Task AddEmotionAsync(AddEmotionCommand command, CancellationToken cancellationToken = default);
    Task RemoveEmotionAsync(RemoveEmotionCommand command, CancellationToken cancellationToken = default);
}