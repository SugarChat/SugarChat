using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
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
    }
}
