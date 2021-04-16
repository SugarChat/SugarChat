using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace SugarChat.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((hostingContext, builder) =>
                    {
                        var env = hostingContext.HostingEnvironment;
                        builder.SetBasePath(env.ContentRootPath)
                               .AddJsonFile("appsettings.json", false, true)
                               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                               .AddEnvironmentVariables();

                    });

                    webBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
                                    loggerConfiguration
                                    .ReadFrom
                                    .Configuration(hostingContext.Configuration)
                                    .Enrich
                                    .WithProperty("ReleaseVersion", hostingContext.Configuration.GetValue<string>("ReleaseVersion")))
                              .UseStartup<Startup>();
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory());

    }
}