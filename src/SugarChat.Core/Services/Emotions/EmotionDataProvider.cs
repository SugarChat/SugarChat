using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Exceptions;

namespace SugarChat.Core.Services.Emotions
{
    public class EmotionDataProvider : IEmotionDataProvider
    {
        private readonly IRepository _repository;

        public EmotionDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Emotion>> GetByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<Emotion>(o => ids.Contains(o.Id), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Emotion>> GetByUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<Emotion>(o => o.UserId == userId, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task AddAsync(Emotion emotion, CancellationToken cancellation = default)
        {
            int affectedLineNum = await _repository.AddAsync(emotion, cancellation).ConfigureAwait(false);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(Prompt.AddEmotionFailed.WithParams(emotion.Id));
            }
        }

        public async Task RemoveAsync(Emotion emotion, CancellationToken cancellation = default)
        {
            int affectedLineNum = await _repository.RemoveAsync(emotion, cancellation).ConfigureAwait(false);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(Prompt.RemoveEmotionFailed.WithParams(emotion.Id));
            }
        }
    }
}