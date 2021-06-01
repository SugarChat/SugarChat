using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ServiceStack.Redis;
using SugarChat.Push.SignalR.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger Logger;
        private readonly IRedisClient _redis;

        public ChatHub(ILoggerFactory loggerFactory, IRedisClient redis)
        {
            Logger = loggerFactory.CreateLogger<ChatHub>();
            _redis = redis;
        }

        public override Task OnConnectedAsync()
        {
            // todo 
            Logger.LogInformation(Context.ConnectionId + ":" + Context.UserIdentifier + ":" + "Online");
            var connectionkey = Context.GetHttpContext().Request.Query["connectionkey"].ToString();
            var userinfo = _redis.Get<UserInfoModel>("Connectionkey:" + connectionkey);
            _redis.Set("Connectionkey:" + connectionkey, userinfo);
            var connectionIds = _redis.Get<List<string>>("UserConnectionIds:" + Context.UserIdentifier);
            if(connectionIds is null)
            {
                connectionIds = new List<string>();
            }
            connectionIds.Add(Context.ConnectionId);
            _redis.Set("UserConnectionIds:" + Context.UserIdentifier, connectionIds, TimeSpan.Zero);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            // todo 
            Logger.LogInformation(Context.ConnectionId + ":" + Context.UserIdentifier + ":" + "Offline");
            var connectionkey = Context.GetHttpContext().Request.Query["connectionkey"].ToString();
            var userinfo = _redis.Get<UserInfoModel>("Connectionkey:" + connectionkey);
            _redis.Set("Connectionkey:" + connectionkey, userinfo, TimeSpan.FromMinutes(5));
            var connectionIds = _redis.Get<List<string>>("UserConnectionIds:" + Context.UserIdentifier);
            connectionIds.Remove(Context.ConnectionId);
            _redis.Set("UserConnectionIds:" + Context.UserIdentifier, connectionIds);
            return base.OnDisconnectedAsync(exception);
        }
    }

    
}
