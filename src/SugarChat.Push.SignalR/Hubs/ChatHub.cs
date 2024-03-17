using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ServiceStack.Redis;
using SugarChat.Push.SignalR.Models;
using SugarChat.Push.SignalR.Services;
using SugarChat.Push.SignalR.Services.Caching;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Security.Policy;
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
        private readonly ICacheService _cacheService;

        public ChatHub(ILoggerFactory loggerFactory, IRedisClient redis, ICacheService cacheService)
        {
            Logger = loggerFactory.CreateLogger<ChatHub>();
            _redis = redis;
            _cacheService = cacheService;
        }

        public override async Task OnConnectedAsync()
        {
            SemaphoreSlim semaphore = UserLock.UserSemaphores.GetOrAdd(Context.UserIdentifier, _ => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();
            // todo 
            var connectionkey = Context.GetHttpContext().Request.Query["connectionkey"].ToString();
            if (string.IsNullOrWhiteSpace(connectionkey))
            {
                throw new HubException("Unauthorized Access", new UnauthorizedAccessException());
            }

            var userinfo = await _cacheService.GetByKeyFromRedis<UserInfoModel>("Connectionkey:" + connectionkey);
            //var userinfo = _redis.Get<UserInfoModel>("Connectionkey:" + connectionkey);
            if (userinfo is null)
            {
                throw new HubException("Unauthorized Access", new UnauthorizedAccessException());
            }
            Logger.LogInformation(Context.ConnectionId + ":" + Context.UserIdentifier + ":" + "Online");
            await _cacheService.SetRedisByKey("Connectionkey:" + connectionkey, userinfo, TimeSpan.FromDays(1));
            //_redis.Set("Connectionkey:" + connectionkey, userinfo);
            await SetUserConnectionId().ConfigureAwait(false);
            await base.OnConnectedAsync();
            semaphore.Release();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // todo 
            var connectionkey = Context.GetHttpContext().Request.Query["connectionkey"].ToString();
            if (string.IsNullOrWhiteSpace(connectionkey))
            {
                return;
            }
            var userinfo = await _cacheService.GetByKeyFromRedis<UserInfoModel>("Connectionkey:" + connectionkey);
            //var userinfo = _redis.Get<UserInfoModel>("Connectionkey:" + connectionkey);
            if (userinfo is null)
            {
                return;
            }
            Logger.LogInformation(Context.ConnectionId + ":" + Context.UserIdentifier + ":" + "Offline");
            await _cacheService.SetRedisByKey("Connectionkey:" + connectionkey, userinfo, TimeSpan.FromDays(1));
            //_redis.Set("Connectionkey:" + connectionkey, userinfo, TimeSpan.FromMinutes(5));
            await RemoveUserConnectionId().ConfigureAwait(false);
            await base.OnDisconnectedAsync(exception);
        }

        private async Task SetUserConnectionId()
        {
            var redisValue = await _cacheService.GetHashByKeyFromRedis<string>("UserConnectionIds", Context.UserIdentifier).ConfigureAwait(false);
            //var redisValue = _redis.GetValueFromHash("UserConnectionIds", Context.UserIdentifier);

            var connectionIds = new List<string>();
            if (redisValue is not null)
            {
                connectionIds = JsonSerializer.Deserialize<List<string>>(redisValue);
            }
            connectionIds.Add(Context.ConnectionId);
            await _cacheService.SetHashRedisByKey("UserConnectionIds", Context.UserIdentifier, JsonSerializer.Serialize(connectionIds));
            //_redis.SetEntryInHash("UserConnectionIds", Context.UserIdentifier, JsonSerializer.Serialize(connectionIds));
        }
        private async Task RemoveUserConnectionId()
        {
            var connectionIds = JsonSerializer.Deserialize<List<string>>(_redis.GetValueFromHash("UserConnectionIds", Context.UserIdentifier));

            connectionIds.Remove(Context.ConnectionId);
            await _cacheService.SetHashRedisByKey("UserConnectionIds", Context.UserIdentifier, JsonSerializer.Serialize(connectionIds));
            //_redis.SetEntryInHash("UserConnectionIds", Context.UserIdentifier, JsonSerializer.Serialize(connectionIds));
        }
    }


}
