using Mediator.Net;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Core.Basic;
using SugarChat.Message.Commands;
using SugarChat.Message.Commands.Message;
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
            var response = await _mediator.SendAsync<SendMessageCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("revoke"), HttpPost]
        public async Task<IActionResult> RevokeMessage(RevokeMessageCommand command)
        {
            var response = await _mediator.SendAsync<RevokeMessageCommand, SugarChatResponse>(command);
            return Ok(response);
        }
    }
}
