using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ServiceStack.Redis;
using SugarChat.Push.SignalR.Models;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Services
{
    public class ConnectService : IConnectService
    {
        private readonly IRedisClient _redis;
        public ConnectService(IConfiguration configuration, IHostEnvironment environment, IRedisClient redis)
        {
            Configuration = configuration;
            Env = environment;
            _redis = redis;
        }

        private IConfiguration Configuration;
        private IHostEnvironment Env;

        public Task<string> GetConnectionUrl(string userIdentifier)
        {
            var baseUrl = Environment.GetEnvironmentVariable("SUGARCHAT_SIGNAL_HUB_URL");
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                baseUrl = Configuration.GetSection("SUGARCHAT_SIGNAL_HUB_URL").Value;
            }
            var key = Guid.NewGuid().ToString("N");
            _redis.Set("Connectionkey:" + key, new UserInfoModel { Identifier = userIdentifier }, TimeSpan.FromMinutes(5));
            return Task.FromResult($"{baseUrl}?connectionkey={key}");
        }
    }
}
