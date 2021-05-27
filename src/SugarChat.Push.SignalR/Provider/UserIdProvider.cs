using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using ServiceStack.Redis;
using SugarChat.Push.SignalR.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SugarChat.Push.SignalR.Provider
{
    public class UserIdProvider : IUserIdProvider
    {
        private IHttpContextAccessor _httpContextAccessor;
        private IRedisClient _redis;

        public UserIdProvider(IHttpContextAccessor httpContextAccessor, IRedisClient redis)
        {
            _httpContextAccessor = httpContextAccessor;
            _redis = redis;
        }

        public string GetUserId(HubConnectionContext connection)
        {
            var key = _httpContextAccessor.HttpContext.Request.Query["connectionkey"].ToString();
            var v = _redis.Get<UserInfoModel>("Connectionkey:" + key);
            if(v is null)
            {
                return null;
            }
            return v.Identifier;
        }
    }
}
