using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.Users;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Users
{
    public class AddUserCommandHandler : ICommandHandler<AddUserCommand, SugarChatResponse>
    {
        private readonly IUserService _userService;
        public AddUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<AddUserCommand> context, CancellationToken cancellationToken)
        {
            var userAddedEvent = await _userService.AddUserAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(userAddedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();

        }
    }
}
