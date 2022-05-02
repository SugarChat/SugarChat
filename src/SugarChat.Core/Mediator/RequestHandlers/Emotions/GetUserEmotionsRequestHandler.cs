using System.Threading;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Emotions;
using SugarChat.Message.Requests.Emotions;
using SugarChat.Message.Responses.Emotions;

namespace SugarChat.Core.Mediator.RequestHandlers.Emotions
{
    public class GetUserEmotionsRequestHandler : IRequestHandler<GetUserEmotionsRequest, GetUserEmotionsResponse>
    {
        private readonly IEmotionService _emotionService;

        public GetUserEmotionsRequestHandler(IEmotionService emotionService)
        {
            _emotionService = emotionService;
        }

        public async Task<GetUserEmotionsResponse> Handle(IReceiveContext<GetUserEmotionsRequest> context, CancellationToken cancellationToken)
        {
            return await _emotionService.GetUserEmotionsAsync(context.Message, cancellationToken);
        }
    }
}
