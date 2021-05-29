using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Users
{
    public class GetUserProfileRequestHandler : IRequestHandler<GetUserRequest, GetUserResponse>
    {
        private readonly IUserService _userService;

        public GetUserProfileRequestHandler(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<GetUserResponse> Handle(IReceiveContext<GetUserRequest> context, CancellationToken cancellationToken)
        {
            return await _userService.GetUserAsync(context.Message, cancellationToken).ConfigureAwait(false);
        }
    }
}
