using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Requests;
using SugarChat.Message.Dtos;
using SugarChat.Message.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Basic;

namespace SugarChat.Core.Mediator.RequestHandlers.Users
{
    public class GetFriendsOfUserRequestHandler : IRequestHandler<GetFriendsOfUserRequest, SugarChatResponse<PagedResult<UserDto>>>
    {
        private readonly IUserService _userService;

        public GetFriendsOfUserRequestHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<SugarChatResponse<PagedResult<UserDto>>> Handle(IReceiveContext<GetFriendsOfUserRequest> context, CancellationToken cancellationToken)
        {
            var response = await _userService.GetFriendsOfUserAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<PagedResult<UserDto>>() { Data = response.Friends };
        }
    }
}
