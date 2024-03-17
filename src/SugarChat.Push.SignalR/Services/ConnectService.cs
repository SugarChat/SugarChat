using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ServiceStack.Redis;
using SugarChat.Push.SignalR.Models;
using SugarChat.Push.SignalR.Services.Caching;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Services
{
    public class ConnectService : IConnectService
    {
        private readonly IRedisClient _redis;
        private readonly ICacheService _cacheService;

        public ConnectService(IConfiguration configuration, IHostEnvironment environment, IRedisClient redis, ICacheService cacheService)
        {
            Configuration = configuration;
            Env = environment;
            _redis = redis;
            _cacheService = cacheService;
        }

        private IConfiguration Configuration;
        private IHostEnvironment Env;

        public async Task<string> GetConnectionUrl(string userIdentifier, bool isInterior = false)
        {
            string baseUrl = "";
            if (isInterior)
            {
                baseUrl = Environment.GetEnvironmentVariable("SUGARCHAT_SIGNAL_HUB_INTERIOR_URL");
                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    baseUrl = Configuration.GetSection("SUGARCHAT_SIGNAL_HUB_INTERIOR_URL").Value;
                }
            }
            else
            {
                baseUrl = Environment.GetEnvironmentVariable("SUGARCHAT_SIGNAL_HUB_URL");
                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    baseUrl = Configuration.GetSection("SUGARCHAT_SIGNAL_HUB_URL").Value;
                }
            }
            
            var key = Guid.NewGuid().ToString("N");
            await _cacheService.SetRedisByKey("Connectionkey:" + key, new UserInfoModel { Identifier = userIdentifier }, TimeSpan.FromDays(1));
            //_redis.Set("Connectionkey:" + key, new UserInfoModel { Identifier = userIdentifier }, TimeSpan.FromMinutes(5));
            return $"{baseUrl}?connectionkey={key}";
        }
    }
}
