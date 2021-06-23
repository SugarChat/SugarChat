using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using SugarChat.Push.SignalR.Cache;
using SugarChat.Push.SignalR.Provider;
using SugarChat.Push.SignalR.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SugarChat.Push.SignalR.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static ISignalRServerBuilder AddSugarChatSignalR(this IServiceCollection services)
        {
            services.AddScoped<IConnectService, ConnectService>();
            services.AddScoped<IChatHubService, ChatHubService>();
            services.AddSingleton<IUserIdProvider, UserIdProvider>();
            return services.AddSignalR();
        }
        public static IServiceCollection UseRedis(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(connectionString));
            services.AddSingleton<ICacheService, RedisCacheService>();
            return services;
        }
        public static IServiceCollection UseMemoryCache(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            return services;
        }
    }
}
