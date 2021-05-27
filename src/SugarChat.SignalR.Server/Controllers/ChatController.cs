using Mediator.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Push.SignalR.Model;
using SugarChat.Push.SignalR.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SugarChat.SignalR.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConnectService _connectService;

        public ChatController(IConnectService connectService, IMediator mediator)
        {
            _connectService = connectService;
            _mediator = mediator;
        }
        [HttpPost("GetConnectionUrl")]
        public async Task<IActionResult> GetConnectionUrl(GetConnectionUrlModel model)
        {
            var url = await _connectService.GetConnectionUrl(model);
            return Ok(url);
        }
    }
}
