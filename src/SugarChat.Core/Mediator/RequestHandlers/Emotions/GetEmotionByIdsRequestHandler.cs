using System.Threading;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Emotions;
using SugarChat.Message.Requests.Emotions;
using SugarChat.Message.Responses.Emotions;

namespace SugarChat.Core.Mediator.RequestHandlers.Emotions
{
    public class GetEmotionByIdsRequestHandler : IRequestHandler<GetEmotionByIdsRequest, GetEmotionByIdsResponse>
    {
        private readonly IEmotionService _emotionService;

        public GetEmotionByIdsRequestHandler(IEmotionService emotionService)
        {
            _emotionService = emotionService;
        }

        public async Task<GetEmotionByIdsResponse> Handle(IReceiveContext<GetEmotionByIdsRequest> context, CancellationToken cancellationToken)
        {
            return await _emotionService.GetEmotionByIdsAsync(context.Message, cancellationToken);
        }
    }
}
