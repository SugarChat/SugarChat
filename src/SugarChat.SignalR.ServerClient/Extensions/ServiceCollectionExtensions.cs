using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Collections.Generic;
using System.Text;

namespace SugarChat.SignalR.ServerClient.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSugarChatSignalRServerHttpClient(this IServiceCollection services, string serverUrl)
        {
            services.AddHttpClient<IServerClient, ServerHttpClient>(client => {
                    client.BaseAddress = new Uri(serverUrl);
                })
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(4),
                    TimeSpan.FromSeconds(8),
                    TimeSpan.FromSeconds(16),
                    TimeSpan.FromSeconds(32)
                }));
            return services;
        }
        public static IServiceCollection AddSugarChatSignalRServerWebSocketClient(this IServiceCollection services, string serverUrl)
        {
            services.AddSingleton<IServerClient, ServerWebScoketClient>(sp => new ServerWebScoketClient(serverUrl));
            return services;
        }
    }
}
