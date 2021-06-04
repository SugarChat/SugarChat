using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ServiceStack.Redis;
using SugarChat.Push.SignalR.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
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
            var connectionkey = Context.GetHttpContext().Request.Query["connectionkey"].ToString();
            if (string.IsNullOrWhiteSpace(connectionkey))
            {
                throw new HubException("Unauthorized Access", new UnauthorizedAccessException());
            }
            var userinfo = _redis.Get<UserInfoModel>("Connectionkey:" + connectionkey);
            if(userinfo is null)
            {
                throw new HubException("Unauthorized Access", new UnauthorizedAccessException());
            }
            Logger.LogInformation(Context.ConnectionId + ":" + Context.UserIdentifier + ":" + "Online");
            _redis.Set("Connectionkey:" + connectionkey, userinfo);
            SetUserConnectionId();
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            // todo 
            var connectionkey = Context.GetHttpContext().Request.Query["connectionkey"].ToString();
            if (string.IsNullOrWhiteSpace(connectionkey))
            {
                return Task.CompletedTask;
            }
            var userinfo = _redis.Get<UserInfoModel>("Connectionkey:" + connectionkey);
            if (userinfo is null)
            {
                return Task.CompletedTask;
            }
            Logger.LogInformation(Context.ConnectionId + ":" + Context.UserIdentifier + ":" + "Offline");
            _redis.Set("Connectionkey:" + connectionkey, userinfo, TimeSpan.FromMinutes(5));
            RemoveUserConnectionId();
            return base.OnDisconnectedAsync(exception);
        }

        private void SetUserConnectionId()
        {
            var redisValue = _redis.GetValueFromHash("UserConnectionIds", Context.UserIdentifier);

            var connectionIds = new List<string>();
            if(redisValue is not null)
            {
                connectionIds = JsonSerializer.Deserialize<List<string>>(redisValue);
            }
            connectionIds.Add(Context.ConnectionId);
            _redis.SetEntryInHash("UserConnectionIds", Context.UserIdentifier, JsonSerializer.Serialize(connectionIds));
        }
        private void RemoveUserConnectionId()
        {
            var connectionIds = JsonSerializer.Deserialize<List<string>>(_redis.GetValueFromHash("UserConnectionIds", Context.UserIdentifier));

            connectionIds.Remove(Context.ConnectionId);
            _redis.SetEntryInHash("UserConnectionIds", Context.UserIdentifier, JsonSerializer.Serialize(connectionIds));
        }
    }

    
}
