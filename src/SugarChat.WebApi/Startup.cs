using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Configuration;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Settings;
using SugarChat.Infrastructure.Repositories;

namespace SugarChat.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDb"));
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var module = new ConfigurationModule(Configuration);
            builder.RegisterModule(module);
            builder.RegisterType<UserRepository>().As<IUserRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILifetimeScope scope)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    var userRepo = scope.Resolve<IUserRepository>();
                    await userRepo.AddAsync(new Core.Domain.User(Guid.NewGuid(), "url", new Core.Domain.RegisterInfo(), "public", "nickName", "selfDesc", Core.Common.UserStatus.Online, "fName", "sName", new DateTime(1994, 11, 17)));
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}