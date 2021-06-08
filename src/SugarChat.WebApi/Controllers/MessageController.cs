using Mediator.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Core.Basic;
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
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> SendMessage(SendMessageCommand command)
        {
            var response = await _mediator.SendAsync<SendMessageCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("revoke"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> RevokeMessage(RevokeMessageCommand command)
        {
            var response = await _mediator.SendAsync<RevokeMessageCommand, SugarChatResponse>(command);
            return Ok(response);
        }
    }
}
