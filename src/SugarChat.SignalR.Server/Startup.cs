using Mediator.Net;
using Mediator.Net.MicrosoftDependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceStack.Redis;
using StackExchange.Redis;
using SugarChat.Push.SignalR.Extensions;
using SugarChat.Push.SignalR.Hubs;
using SugarChat.Push.SignalR.Services.Caching;
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
            services.AddSingleton<IRedisClient, RedisClient>(sp => new RedisClient(Configuration.GetSection("SignalRRedis").Value));
            services.AddHttpContextAccessor();
            services.AddControllers();
            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder.AllowAnyMethod()
                           .AllowAnyHeader()
                           .SetIsOriginAllowed(o => true)
                           .AllowCredentials();
                }));
            services.AddSingleton(sp =>
            {
                var redis = ConnectionMultiplexer.Connect(Configuration.GetSection("SignalRRedis").Value);
                return redis;
            });
            services.AddSingleton<IRedisSafeRunner, RedisSafeRunner>();
            services.AddSingleton<ICacheService, CacheService>();
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

            app.UseCors("CorsPolicy");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/hubs/chat", context =>
                {
                });
                endpoints.MapHub<ApiHub>("/hubs/api");
                endpoints.MapControllers();
            });
        }
    }
}
