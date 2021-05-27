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
        private readonly IChatHubService _chatHubService;
        private readonly IConnectService _connectService;

        public ChatController(IChatHubService chatHubService, IConnectService connectService)
        {
            _chatHubService = chatHubService;
            _connectService = connectService;
        }
        [HttpPost("GetConnectionUrl")]
        public async Task<IActionResult> GetConnectionUrl(GetConnectionUrlModel model)
        {
            var url = await _connectService.GetConnectionUrl(model);
            return Ok(url);
        }
    }
}
