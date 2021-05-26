using Autofac;
using Autofac.Extensions.DependencyInjection;
using Mediator.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson.Serialization;
using SugarChat.Core;
using SugarChat.Core.Autofac;
using SugarChat.Core.Mediator.CommandHandler;
using SugarChat.Message.Command;
using System.Reflection;
using SugarChat.Data.MongoDb;
using SugarChat.Data.MongoDb.Autofac;
using SugarChat.Data.MongoDb.Settings;

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
            services.AddControllers();
            services.AddOptions();
            this.services = services;
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //builder.Populate(services);
            builder.RegisterMongoDbRepository(() => Configuration.GetSection("MongoDb"));
            builder.RegisterModule(new SugarChatModule(new Assembly[]
            {
                typeof(SugarChat.Core.Services.IService).Assembly
            }));
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
                endpoints.MapControllers();
            });
        }
    }
}