using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Emotions;
using SugarChat.Message.Basic;
using SugarChat.Message.Dtos.Emotions;
using SugarChat.Message.Requests.Emotions;

namespace SugarChat.Core.Mediator.RequestHandlers.Emotions
{
    public class GetUserEmotionsRequestHandler : IRequestHandler<GetUserEmotionsRequest, SugarChatResponse<IEnumerable<EmotionDto>>>
    {
        private readonly IEmotionService _emotionService;

        public GetUserEmotionsRequestHandler(IEmotionService emotionService)
        {
            _emotionService = emotionService;
        }

        public async Task<SugarChatResponse<IEnumerable<EmotionDto>>> Handle(IReceiveContext<GetUserEmotionsRequest> context, CancellationToken cancellationToken)
        {
            var response = await _emotionService.GetUserEmotionsAsync(context.Message, cancellationToken);
            return new SugarChatResponse<IEnumerable<EmotionDto>>() { Data = response };
        }
    }
}
