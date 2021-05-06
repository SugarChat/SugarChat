using Microsoft.AspNetCore.SignalR;
using SugarChat.Push.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SugarChat.Push.SignalR.Services
{
    public class ChatHubService : IChatHubService
    {
        private readonly IHubContext<ChatHub> _chatHubContext;
    }
}
