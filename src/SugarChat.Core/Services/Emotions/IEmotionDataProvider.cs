using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Message.Paging;

namespace SugarChat.Core.Services.Emotions
{
    public interface IEmotionDataProvider : IDataProvider
    {
        Task<IEnumerable<Emotion>> GetByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
        Task<IEnumerable<Emotion>> GetByUserAsync(string userId, CancellationToken cancellationToken = default);
        Task AddAsync(Emotion emotion, CancellationToken cancellation = default);
        Task RemoveAsync(Emotion emotion, CancellationToken cancellation = default);
    }
}