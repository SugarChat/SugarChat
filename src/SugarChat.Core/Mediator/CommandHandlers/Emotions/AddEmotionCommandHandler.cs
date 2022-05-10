using System.Threading;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Emotions;
using SugarChat.Message.Commands.Emotions;

namespace SugarChat.Core.Mediator.CommandHandlers.Emotions
{
    public class AddEmotionCommandHandler : ICommandHandler<AddEmotionCommand>
    {
        private readonly IEmotionService _emotionService;

        public AddEmotionCommandHandler(IEmotionService emotionService)
        {
            _emotionService = emotionService;
        }

        public async Task Handle(IReceiveContext<AddEmotionCommand> context, CancellationToken cancellationToken)
        {
            await _emotionService.AddEmotionAsync(context.Message, cancellationToken);
        }
    }
}
