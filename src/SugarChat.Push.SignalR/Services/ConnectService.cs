using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ServiceStack.Redis;
using SugarChat.Push.SignalR.Cache;
using SugarChat.Push.SignalR.Models;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Services
{
    public class ConnectService : IConnectService
    {
        private readonly ICacheService _cache;
        public ConnectService(IConfiguration configuration, IHostEnvironment environment, ICacheService cache)
        {
            Configuration = configuration;
            Env = environment;
            _cache = cache;
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
            await _cache.SetAsync("Connectionkey:" + key, new UserInfoModel { Identifier = userIdentifier }, TimeSpan.FromMinutes(5)).ConfigureAwait(false);
            return $"{baseUrl}?connectionkey={key}";
        }
    }
}
