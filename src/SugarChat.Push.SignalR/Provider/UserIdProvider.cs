using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using ServiceStack.Redis;
using SugarChat.Push.SignalR.Cache;
using SugarChat.Push.SignalR.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SugarChat.Push.SignalR.Provider
{
    public class UserIdProvider : IUserIdProvider
    {
        private IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheService _cache;

        public UserIdProvider(IHttpContextAccessor httpContextAccessor, ICacheService cache)
        {
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
        }

        public string GetUserId(HubConnectionContext connection)
        {
            var key = _httpContextAccessor.HttpContext.Request.Query["connectionkey"].ToString();
            var v = _cache.Get<UserInfoModel>("Connectionkey:" + key);
            if (v is null)
            {
                return null;
            }
            return v.Identifier;
        }
    }
}
