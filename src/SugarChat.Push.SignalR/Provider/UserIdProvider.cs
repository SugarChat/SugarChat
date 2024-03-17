using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using ServiceStack.Redis;
using SugarChat.Push.SignalR.Models;
using SugarChat.Push.SignalR.Services.Caching;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Provider
{
    public class UserIdProvider : IUserIdProvider
    {
        private IHttpContextAccessor _httpContextAccessor;
        private IRedisClient _redis;
        private readonly ICacheService _cacheService;

        public UserIdProvider(IHttpContextAccessor httpContextAccessor, IRedisClient redis, ICacheService cacheService)
        {
            _httpContextAccessor = httpContextAccessor;
            _redis = redis;
            _cacheService = cacheService;
        }

        public string GetUserId(HubConnectionContext connection)
        {
            var key = _httpContextAccessor.HttpContext.Request.Query["connectionkey"].ToString();
            var v = _cacheService.GetByKeyFromRedis<UserInfoModel>("Connectionkey:" + key).Result;
            //var v = _redis.Get<UserInfoModel>("Connectionkey:" + key);
            if(v is null)
            {
                return null;
            }
            return v.Identifier;
        }
    }
}
