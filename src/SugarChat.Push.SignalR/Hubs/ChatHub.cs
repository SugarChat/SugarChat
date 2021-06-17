using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ServiceStack.Redis;
using SugarChat.Push.SignalR.Cache;
using SugarChat.Push.SignalR.Const;
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
        private readonly ICacheService _cache;

        public ChatHub(ILoggerFactory loggerFactory, ICacheService cache)
        {
            Logger = loggerFactory.CreateLogger<ChatHub>();
            _cache = cache;
        }

        public override async Task OnConnectedAsync()
        {
            var connectionkey = Context.GetHttpContext().Request.Query["connectionkey"].ToString();
            if (string.IsNullOrWhiteSpace(connectionkey))
            {
                throw new HubException("Unauthorized Access", new UnauthorizedAccessException());
            }
            var userinfo = _cache.Get<UserInfoModel>(CacheKey.Connectionkey + ":" + connectionkey);
            if(userinfo is null)
            {
                throw new HubException("Unauthorized Access", new UnauthorizedAccessException());
            }
            Logger.LogInformation(Context.ConnectionId + ":" + Context.UserIdentifier + ":" + "Online");
            _cache.Set(CacheKey.Connectionkey + ":" + connectionkey, userinfo);
            await SetUserConnectionId();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionkey = Context.GetHttpContext().Request.Query["connectionkey"].ToString();
            if (string.IsNullOrWhiteSpace(connectionkey))
            {
                return;
            }
            var userinfo = _cache.Get<UserInfoModel>(CacheKey.Connectionkey + ":" + connectionkey);
            if (userinfo is null)
            {
                return;
            }
            Logger.LogInformation(Context.ConnectionId + ":" + Context.UserIdentifier + ":" + "Offline");
            _cache.Set(CacheKey.Connectionkey + ":" + connectionkey, userinfo, TimeSpan.FromMinutes(5));
            await RemoveUserConnectionId();
        }

        private async Task SetUserConnectionId()
        {
            var connectionIds = await _cache.HashGetAsync<List<string>> (CacheKey.UserConnectionIds, Context.UserIdentifier).ConfigureAwait(false);

            if(connectionIds is null)
            {
                connectionIds = new List<string>();
            }
            connectionIds.Add(Context.ConnectionId);
            await _cache.HashSetAsync(CacheKey.UserConnectionIds, Context.UserIdentifier, connectionIds).ConfigureAwait(false);
        }
        private async Task RemoveUserConnectionId()
        {
            var connectionIds = await _cache.HashGetAsync<List<string>>(CacheKey.UserConnectionIds, Context.UserIdentifier).ConfigureAwait(false);
            connectionIds.Remove(Context.ConnectionId);
            await _cache.HashSetAsync(CacheKey.UserConnectionIds, Context.UserIdentifier, connectionIds).ConfigureAwait(false);
        }
    }

    
}
