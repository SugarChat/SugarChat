using Mediator.Net;
using Mediator.Net.MicrosoftDependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceStack.Redis;
using SugarChat.Push.SignalR.Extensions;
using SugarChat.Push.SignalR.Hubs;
using System.Reflection;

namespace SugarChat.SignalR.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IRedisClient, RedisClient>(sp => new RedisClient(Configuration.GetSection("Redis").Value));
            services.AddHttpContextAccessor();
            services.AddControllers();
            services.AddSugarChatSignalR()
                .AddJsonProtocol()
                .AddStackExchangeRedis(Configuration.GetSection("SignalRRedis").Value);

            #region ConfigureMediator
            var mediatorBuilder = new MediatorBuilder();
            mediatorBuilder.RegisterHandlers(
                typeof(SugarChat.Push.SignalR.Hubs.ChatHub).Assembly
            );
            services.RegisterMediator(mediatorBuilder);
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/hubs/chat", context =>
                {
                });
                endpoints.MapControllers();
            });
        }
    }
}
