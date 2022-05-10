using System.Threading;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Emotions;
using SugarChat.Message.Commands.Emotions;

namespace SugarChat.Core.Mediator.CommandHandlers.Emotions
{
    public class RemoveEmotionCommandHandler : ICommandHandler<RemoveEmotionCommand>
    {
        private readonly IEmotionService _emotionService;

        public RemoveEmotionCommandHandler(IEmotionService emotionService)
        {
            _emotionService = emotionService;
        }

        public async Task Handle(IReceiveContext<RemoveEmotionCommand> context, CancellationToken cancellationToken)
        {
            await _emotionService.RemoveEmotionAsync(context.Message, cancellationToken);
        }
    }
}
