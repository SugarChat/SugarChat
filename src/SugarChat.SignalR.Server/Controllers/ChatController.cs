using Mediator.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SugarChat.Push.SignalR.Mediator.Goup;
using SugarChat.Push.SignalR.Mediator.SendMessage;
using SugarChat.Push.SignalR.Models;
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
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IMediator _mediator;
        private readonly IConnectService _connectService;
        private static string ServerKey;
        private static bool Security;

        public ChatController(IConnectService connectService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _connectService = connectService;
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
            ServerKey = configuration.GetSection("ServerClientKey").Value;
            Security = configuration.GetValue<bool>("Security");
        }
        [HttpGet("ConnectionUrl")]
        public async Task<IActionResult> GetConnectionUrl(string userIdentifier)
        {
            if (!Check())
            {
                return Unauthorized();
            }
            var url = await _connectService.GetConnectionUrl(userIdentifier);
            return Ok(url);
        }

        [HttpPost("Group")]
        public async Task<IActionResult> Group(GroupActionModel model)
        {
            if (!Check())
            {
                return Unauthorized();
            }
            await _mediator.SendAsync(new GroupCommand { Action = model.Action, GroupName = model.GroupName, UserIdentifier = model.UserIdentifier });
            return Ok();
        }

        [HttpPost("Message")]
        public async Task<IActionResult> SendMessage(SendMessageModel model)
        {
            if (!Check())
            {
                return Unauthorized();
            }
            var command = new SendMessageCommand { SendTo = model.SendTo, Messages = model.Messages, SendWay = model.SendWay };
            switch (model.SendWay)
            {
                case Push.SignalR.SendWay.User:
                    command.Method = "SendUserMessage";
                    break;
                case Push.SignalR.SendWay.Group:
                    command.Method = "SendGroupMessage";
                    break;
                case Push.SignalR.SendWay.All:
                    command.Method = "SendAllMessage";
                    break;
                default:
                    break;
            }
            await _mediator.SendAsync(command);
            return Ok();
        }

        [HttpPost("MassMessage")]
        public async Task<IActionResult> SendMassMessage(SendMassMessageModel model)
        {
            if (!Check())
            {
                return Unauthorized();
            }
            var command = new SendMessageCommand { SendTos = model.SendTos, Messages = model.Messages, SendWay = model.SendWay };
            switch (model.SendWay)
            {
                case Push.SignalR.SendWay.User:
                    command.Method = "SendMassUserMessage";
                    break;
                case Push.SignalR.SendWay.Group:
                    command.Method = "SendMassGroupMessage";
                    break;
                default:
                    break;
            }
            await _mediator.SendAsync(command);
            return Ok();
        }

        [HttpPost("CustomMessage")]
        public async Task<IActionResult> SendCustomMessage(SendCustomMessageModel model)
        {
            if (!Check())
            {
                return Unauthorized();
            }
            var command = new SendMessageCommand { Method = model.Method, SendTo = model.SendTo, Messages = model.Messages, SendWay = model.SendWay };
           
            await _mediator.SendAsync(command);
            return Ok();
        }

        [HttpPost("MassCustomMessage")]
        public async Task<IActionResult> SendMassCustomMessage(SendMassCustomMessageModel model)
        {
            if (!Check())
            {
                return Unauthorized();
            }
            var command = new SendMessageCommand { Method = model.Method, SendTos = model.SendTos, Messages = model.Messages, SendWay = model.SendWay };
            
            await _mediator.SendAsync(command);
            return Ok();
        }
        private bool Check()
        {
            var securityKey = _httpContextAccessor.HttpContext.Request.Query["security"].ToString();
            if (Security && (string.IsNullOrWhiteSpace(securityKey) || securityKey != ServerKey))
            {
                return false;
            }
            return true;
        }
    }
}
