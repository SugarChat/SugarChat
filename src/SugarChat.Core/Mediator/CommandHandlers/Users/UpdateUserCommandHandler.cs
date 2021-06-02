using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.Users;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Users
{
    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
    {
        public IUserService _userService;
        public UpdateUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public Task Handle(IReceiveContext<UpdateUserCommand> context, CancellationToken cancellationToken)
        {
            return _userService.UpdateUserAsync(context.Message, cancellationToken);

        }
    }
}
