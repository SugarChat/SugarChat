using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;

namespace SugarChat.WebApi
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace;

        public static int Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.Development.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            var seq = Environment.GetEnvironmentVariable("Serilog.Seq");
            if (string.IsNullOrEmpty(seq))
            {
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .Enrich.WithProperty("ReleaseVersion", configuration.GetValue<string>("ReleaseVersion"))
                    .CreateLogger();
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", "SugarChat.WebApi")
                    .Enrich.WithProperty("ReleaseVersion", configuration.GetValue<string>("ReleaseVersion"))
                    .WriteTo.Seq(Environment.GetEnvironmentVariable("Serilog.Seq"), apiKey: Environment.GetEnvironmentVariable("Serilog.Seq.Apikey"))
                    .CreateLogger();
            }

            try
            {
                Log.Information("Configuring web host ({ApplicationContext})...", AppName);
                var webHost = CreateHostBuilder(args).Build();

                Log.Information("Starting web host ({ApplicationContext})...", AppName);
                webHost.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder.UseStartup<Startup>()
                        .UseSerilog();
                });

    }
}