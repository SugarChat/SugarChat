using Mediator.Net;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Message.Command;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Commands.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SugarChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MessageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("send"), HttpPost]
        public async Task<IActionResult> SendMessage(SendMessageCommand command)
        {
            await _mediator.SendAsync(command);
            return Ok();
        }

        [Route("revoke"), HttpPost]
        public async Task<IActionResult> RevokeMessage(RevokeMessageCommand command)
        {
            await _mediator.SendAsync(command);
            return Ok();
        }
    }
}
