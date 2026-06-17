using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SugarChat.Core.Autofac;
using System.Reflection;
using SugarChat.Data.MongoDb.Autofac;
using SugarChat.Core.Services;
using SugarChat.SignalR.ServerClient.Extensions;
using System.IO;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using SugarChat.Core.Services.Users;
using SugarChat.Core.Cache;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SugarChat.Core;
using SugarChat.Message;
using Hangfire;
using Hangfire.MemoryStorage;

namespace SugarChat.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        private IServiceCollection services;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSugarChatSignalRServerHttpClient(Configuration["SignalR:ServerUrl"]);
            services.AddControllers();
            services.AddOptions();
            this.services = services;
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SugarChat WebApi Document",
                    Version = "v1"
                });

                var securityRequirement = new OpenApiSecurityRequirement
                    {
                        {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "BearerAuth"
                                    }
                                },
                                new string[] {}
                        }
                    };

                c.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme
                {
                    Description = "Usage:Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(securityRequirement);

                var xmlName = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlName);
                c.IncludeXmlComments(xmlPath, true);
            });
            services.AddHttpContextAccessor();
            services.AddHangfire(x => x.UseStorage(new MemoryStorage()));
            services.AddHangfireServer();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //builder.Populate(services);
            builder.RegisterMongoDbRepository(() => Configuration.GetSection("MongoDb"));
            builder.RegisterModule(new SugarChatModule(new Assembly[]
            {
                typeof(IService).Assembly
            }, new RunTimeProvider(RunTimeType.AspNetCoreApi)));
            builder.RegisterBuildCallback(lifetimeScope => SetAllUserCache(lifetimeScope));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.ShowExtensions();
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SugarChat WebApi v1");
                c.RoutePrefix = string.Empty;
            });
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private async Task SetAllUserCache(ILifetimeScope lifetimeScope)
        {
            var userDataProvider = lifetimeScope.Resolve<IUserDataProvider>();
            var memoryCache = lifetimeScope.Resolve<IMemoryCache>();
            var userList = await userDataProvider.GetListAsync();
            memoryCache.Set(CacheService.AllUser, userList, new DateTimeOffset(new DateTime(2099, 12, 31)));
        }
    }
}
