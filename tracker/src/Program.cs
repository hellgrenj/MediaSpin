using System;
using System.Runtime.Loader;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Hosting;
using tracker.Domain.Ports.Inbound;
using tracker.Domain.Ports.Outbound;
using tracker.Domain.Services;
using tracker.Infrastructure;
using tracker.Persistence;

namespace tracker
{
    class Program
    {
        private static IPipeline _rabbitClient;

        static void Main(string[] args)
        {
            // Setup Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File($"/logs/tracker.log", rollingInterval: RollingInterval.Day)
                .WriteTo.Console()
                .CreateLogger();

            // Setup DI
            var services = new ServiceCollection();
            services.AddLogging(c => c.AddSerilog());
            services.AddNodeServices(options => { }); // replace with Node-grpc microservice (grpc med polly?)
            services.AddSingleton<IPipeline, RabbitClient>();
            services.AddSingleton<IArticlesFileWriter, ArticlesFileWriter>();
            services.AddSingleton<ISpider, Spider>();
            services.AddSingleton<ITracker, Tracker>();
            services.AddSingleton<IValidator, Validator>();
            services.AddSingleton<JobRunner>();
            services.AddSingleton<IExtractor, Extractor>();
            var serviceProvider = services.BuildServiceProvider();
            _rabbitClient = serviceProvider.GetRequiredService<IPipeline>();
            var jobRunner = serviceProvider.GetRequiredService<JobRunner>();
            _rabbitClient.Open(() =>
            {
                jobRunner.StartAsync();
            });

            // Handle cancel press and SIGTERM 
            AssemblyLoadContext.Default.Unloading += SigTermEventHandler;
            Console.CancelKeyPress += CancelHandler;
        }
        private static void SigTermEventHandler(AssemblyLoadContext obj)
        {
            Log.Information("Unloading...");
            _rabbitClient.Close();
        }
        private static void CancelHandler(object sender, ConsoleCancelEventArgs e)
        {
            Log.Information("Exiting...");
            _rabbitClient.Close();
        }
    }
}
