using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using SugarChat.Push.SignalR.Provider;
using System;
using System.Collections.Generic;
using System.Text;

namespace SugarChat.Push.SignalR.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static ISignalRServerBuilder AddSugarChatSignalR(this IServiceCollection services)
        {
            services.AddSingleton<IUserIdProvider, UserIdProvider>();
            return services.AddSignalR();
        }
    }
}
