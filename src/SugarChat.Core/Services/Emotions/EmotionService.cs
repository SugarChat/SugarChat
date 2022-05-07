using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Message.Exceptions;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.Emotions;
using SugarChat.Message.Dtos.Emotions;
using SugarChat.Message.Requests.Emotions;

namespace SugarChat.Core.Services.Emotions
{
    public class EmotionService : IEmotionService
    {
        private readonly IMapper _mapper;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IEmotionDataProvider _emotionDataProvider;

        public EmotionService(IMapper mapper, IUserDataProvider userDataProvider, IEmotionDataProvider emotionDataProvider)
        {
            _mapper = mapper;
            _emotionDataProvider = emotionDataProvider;
            _userDataProvider = userDataProvider;
        }

        private async Task<User> GetUserAsync(string id, CancellationToken cancellation = default)
        {
            return await _userDataProvider.GetByIdAsync(id, cancellation).ConfigureAwait(false);
        }

        public async Task<IEnumerable<EmotionDto>> GetEmotionByIdsAsync(GetEmotionByIdsRequest request, CancellationToken cancellationToken = default)
        {
            var emotions = await _emotionDataProvider.GetByIdsAsync(request.Ids, cancellationToken).ConfigureAwait(false);;
            return _mapper.Map<IEnumerable<EmotionDto>>(emotions);
        }

        public async Task<IEnumerable<EmotionDto>> GetUserEmotionsAsync(GetUserEmotionsRequest request, CancellationToken cancellationToken = default)
        {
            User user = await GetUserAsync(request.UserId, cancellationToken).ConfigureAwait(false);
            user.CheckExist(request.UserId);
            
            var emotions = await _emotionDataProvider.GetByUserAsync(request.UserId, cancellationToken).ConfigureAwait(false);;
            return _mapper.Map<IEnumerable<EmotionDto>>(emotions);
        }

        public async Task AddEmotionAsync(AddEmotionCommand command, CancellationToken cancellationToken = default)
        {
            User user = await GetUserAsync(command.UserId, cancellationToken).ConfigureAwait(false);
            user.CheckExist(command.UserId);

            Emotion emotion = _mapper.Map<Emotion>(command);
            await _emotionDataProvider.AddAsync(emotion, cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveEmotionAsync(RemoveEmotionCommand command, CancellationToken cancellationToken = default)
        {
            User user = await GetUserAsync(command.UserId, cancellationToken).ConfigureAwait(false);
            user.CheckExist(command.UserId);

            Emotion emotion = (await _emotionDataProvider.GetByIdsAsync(new []{command.Id}, cancellationToken).ConfigureAwait(false)).FirstOrDefault();
            emotion.CheckExist(command.Id);

            if (emotion.UserId != user.Id)
            {
                throw new BusinessWarningException(Prompt.EmotionNotBelongToUser.WithParams(command.Id, command.UserId));
            }
            
            await _emotionDataProvider.RemoveAsync(emotion, cancellationToken).ConfigureAwait(false);
        }
    }
}