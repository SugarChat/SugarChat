using Mediator.Net;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.Friends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SugarChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FriendController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("add"), HttpPost]
        public async Task<IActionResult> AddFriend(AddFriendCommand command)
        {
            var response = await _mediator.SendAsync<AddFriendCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("remove"), HttpPost]
        public async Task<IActionResult> RemoveFriend(RemoveFriendCommand command)
        {
            var response = await _mediator.SendAsync<RemoveFriendCommand, SugarChatResponse>(command);
            return Ok(response);
        }
    }
}
