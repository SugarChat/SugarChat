using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
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
        private readonly ICacheService _cacheService;

        public UserIdProvider(IHttpContextAccessor httpContextAccessor, ICacheService cacheService)
        {
            _httpContextAccessor = httpContextAccessor;
            _cacheService = cacheService;
        }

        public string GetUserId(HubConnectionContext connection)
        {
            var key = _httpContextAccessor.HttpContext.Request.Query["connectionkey"].ToString();
            var v = _cacheService.GetByKeyFromRedis<UserInfoModel>("Connectionkey:" + key).Result;
            if(v is null)
            {
                return null;
            }
            return v.Identifier;
        }
    }
}
