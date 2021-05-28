using Mediator.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Push.SignalR.Mediator.Goup;
using SugarChat.Push.SignalR.Mediator.SendMessage;
using SugarChat.Push.SignalR.Model;
using SugarChat.Push.SignalR.Services;
using SugarChat.SignalR.Server.Models;
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
        [HttpGet("ConnectionUrl")]
        public async Task<IActionResult> GetConnectionUrl(string userIdentifier)
        {
            var url = await _connectService.GetConnectionUrl(userIdentifier);
            return Ok(url);
        }

        [HttpPost("Group")]
        public async Task<IActionResult> Group(GroupActionModel model)
        {
            await _mediator.SendAsync(new GroupCommand { Action = model.Action, GroupName = model.GroupName, UserIdentifier = model.UserIdentifier });
            return Ok();
        }

        [HttpPost("Message")]
        public async Task<IActionResult> SendMessage(SendMessageModel model)
        {
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
            var command = new SendMessageCommand { Method = model.Method, SendTo = model.SendTo, Messages = model.Messages, SendWay = model.SendWay };
           
            await _mediator.SendAsync(command);
            return Ok();
        }

        [HttpPost("MassCustomMessage")]
        public async Task<IActionResult> SendMassCustomMessage(SendMassCustomMessageModel model)
        {
            var command = new SendMessageCommand { Method = model.Method, SendTos = model.SendTos, Messages = model.Messages, SendWay = model.SendWay };
            
            await _mediator.SendAsync(command);
            return Ok();
        }
    }
}
