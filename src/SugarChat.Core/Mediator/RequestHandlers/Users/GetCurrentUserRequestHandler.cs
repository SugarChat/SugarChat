using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Requests;
using SugarChat.Message.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Users
{
    public class GetCurrentUserRequestHandler : IRequestHandler<GetCurrentUserRequest, SugarChatResponse<UserDto>>
    {
        private readonly IUserService _userService;

        public GetCurrentUserRequestHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<SugarChatResponse<UserDto>> Handle(IReceiveContext<GetCurrentUserRequest> context, CancellationToken cancellationToken)
        {
            var response = await _userService.GetCurrentUserAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<UserDto>() { Data = response.User };
        }
    }
}
