using System;
using System.Runtime.Loader;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Hosting;
using tracker.Domain.Ports.Outbound;
using tracker.Domain.Services;
using tracker.Infrastructure;
using tracker.Infrastructure.Quartz;
using tracker.Persistence;

namespace tracker
{
    class Program
    {
        private static ScheduleService _service;
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
            services.AddSingleton<IValidator, Validator>();
            services.AddSingleton<IExtractor, Extractor>();
            var serviceProvider = services.BuildServiceProvider();
            var spider = serviceProvider.GetRequiredService<ISpider>();
            var validator = serviceProvider.GetRequiredService<IValidator>();
            var extractor = serviceProvider.GetRequiredService<IExtractor>();
            var loggerForTracker = serviceProvider.GetRequiredService<ILogger<Tracker>>();
            var fileWriter = serviceProvider.GetRequiredService<IArticlesFileWriter>();
            _rabbitClient = serviceProvider.GetRequiredService<IPipeline>();
            _rabbitClient.Open();

            // Start Quartz Service
            _service = new ScheduleService();
            _service.Start(spider, _rabbitClient, loggerForTracker, fileWriter, validator, extractor);

            // Handle cancel press and SIGTERM 
            AssemblyLoadContext.Default.Unloading += SigTermEventHandler;
            Console.CancelKeyPress += CancelHandler;

            while (true)
            {
                System.Console.Read();
            };
        }
        private static void SigTermEventHandler(AssemblyLoadContext obj)
        {
            Log.Information("Unloading...");
            _rabbitClient.Close();
            _service.Stop();
        }
        private static void CancelHandler(object sender, ConsoleCancelEventArgs e)
        {
            Log.Information("Exiting...");
            _rabbitClient.Close();
            _service.Stop();
        }
    }
}
