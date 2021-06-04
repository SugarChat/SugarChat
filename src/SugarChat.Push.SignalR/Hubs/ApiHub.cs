using Mediator.Net;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using SugarChat.Push.SignalR.Mediator.Goup;
using SugarChat.Push.SignalR.Mediator.SendMessage;
using SugarChat.Push.SignalR.Models;
using SugarChat.Push.SignalR.Services;
using System;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Hubs
{
    public class ApiHub : Hub
    {
        private readonly IMediator _mediator;
        private readonly IConnectService _connectService;
        private static string ServerKey;
        private static bool Security;

        public ApiHub(IMediator mediator, IConnectService connectService, IConfiguration configuration)
        {
            _mediator = mediator;
            _connectService = connectService;
            ServerKey = configuration.GetSection("ServerClientKey").Value;
            Security = configuration.GetValue<bool>("Security");
        }

        public async Task<string> GetConnectionUrl(string userIdentifier)
        {
            var url = await _connectService.GetConnectionUrl(userIdentifier);
            return url;
        }

        public async Task Group(GroupActionModel model)
        {
            await _mediator.SendAsync(new GroupCommand { Action = model.Action, GroupName = model.GroupName, UserIdentifier = model.UserIdentifier });
        }

        public async Task SendMessage(SendMessageModel model)
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
        }

        public async Task SendMassMessage(SendMassMessageModel model)
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
        }

        public async Task SendCustomMessage(SendCustomMessageModel model)
        {
            var command = new SendMessageCommand { Method = model.Method, SendTo = model.SendTo, Messages = model.Messages, SendWay = model.SendWay };

            await _mediator.SendAsync(command);
        }

        public async Task SendMassCustomMessage(SendMassCustomMessageModel model)
        {
            var command = new SendMessageCommand { Method = model.Method, SendTos = model.SendTos, Messages = model.Messages, SendWay = model.SendWay };

            await _mediator.SendAsync(command);
        }
        public override Task OnConnectedAsync()
        {
            var securityKey = Context.GetHttpContext().Request.Query["security"].ToString();
            if (Security && (string.IsNullOrWhiteSpace(securityKey)|| securityKey!= ServerKey))
            {
                throw new HubException("Unauthorized Access", new UnauthorizedAccessException());
            }
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(System.Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
