using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SugarChat.SignalR.Server.Models;
using SugarChat.SignalR.ServerClient;

namespace SugarChat.WebApi.Controllers
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
        [HttpGet("getConnectionUrl")]
        public async Task<string> GetConnectionUrl(string userIdentifier)
        {
             return await _client.GetConnectionUrl(userIdentifier).ConfigureAwait(false);
        }
    }
}
