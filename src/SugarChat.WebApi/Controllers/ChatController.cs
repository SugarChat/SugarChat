using System.Collections.Generic;
using System.Threading.Tasks;
using Mediator.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Core.Basic;
using SugarChat.Message.Commands.SignalR;
using SugarChat.Message.Requests;
using SugarChat.Message.Requests.SignalR;
using SugarChat.Shared.Dtos;
using SugarChat.Shared.Dtos.GroupUsers;
using SugarChat.SignalR.Enums;
using SugarChat.SignalR.Server.Models;
using SugarChat.SignalR.ServerClient;

namespace SugarChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IServerClient _client;
        private readonly IMediator _mediator;

        public ChatController(IServerClient client, IMediator mediator)
        {
            _client = client;
            _mediator = mediator;
        }

        [HttpGet("GetConnectionUrl")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type:  typeof(SugarChatResponse<string>))]
        public async Task<IActionResult> GetConnectionUrl([FromQuery] GetConnectionUrlRequest request)
        {
            var response = await _mediator.RequestAsync<GetConnectionUrlRequest, SugarChatResponse<string>>(request)
                    .ConfigureAwait(false);
                return Ok(response);
        }

        [HttpGet("addToConversations")]
        public async Task<IActionResult> GetOnline([FromQuery] AddToConversationsCommand command)
        {
            var response = await _mediator.SendAsync<AddToConversationsCommand, SugarChatResponse>(command);
            return Ok(response);
 
        }
    }
}