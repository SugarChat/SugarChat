using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Requests;
using SugarChat.Message.Dtos;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Basic;

namespace SugarChat.Core.Mediator.RequestHandlers.Users
{
    public class GetUserProfileRequestHandler : IRequestHandler<GetUserRequest, SugarChatResponse<UserDto>>
    {
        private readonly IUserService _userService;

        public GetUserProfileRequestHandler(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<SugarChatResponse<UserDto>> Handle(IReceiveContext<GetUserRequest> context, CancellationToken cancellationToken)
        {
            var response = await _userService.GetUserAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<UserDto>() { Data = response.User };
        }
    }
}
