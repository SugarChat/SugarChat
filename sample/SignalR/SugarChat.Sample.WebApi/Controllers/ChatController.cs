using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SugarChat.SignalR.Server.Models;
using SugarChat.SignalR.ServerClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SugarChat.Sample.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IServerClient _client;

        public ChatController(IServerClient client)
        {
            _client = client;
        }
        [HttpGet("GetConnectionUrl")]
        public async Task<string> GetConnectionUrl(string userIdentifier)
        {
             return await _client.GetConnectionUrl(userIdentifier).ConfigureAwait(false);
        }

        [HttpPost("SendMessage")]
        public async Task SendMessage(SendMessageModel model)
        {
            await _client.SendMessage(model).ConfigureAwait(false);
        }
        

        [HttpPost("SendCustomMessage")]
        public async Task CustomMessage(SendCustomMessageModel model)
        {
            await _client.SendCustomMessage(model).ConfigureAwait(false);
        }

        [HttpPost("Group")]
        public async Task GroupAction(GroupActionModel model)
        {
            await _client.Group(model).ConfigureAwait(false);
        }
    }
}
