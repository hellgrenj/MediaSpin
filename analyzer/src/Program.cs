using System;
using System.Runtime.Loader;
using Microsoft.AspNetCore.NodeServices;
using src.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Hosting;
using src.Infrastructure;
using src.Domain.Ports.Inbound;
using src.Domain.Ports.Outbound;
using Microsoft.Extensions.Hosting;


namespace analyzer
{
    public class Program
    {
        private static IRabbitEndpoint _rabbitEndpoint;

        private static IPipeline _pipeline;
        public static void Main(string[] args)
        {
            // Setup Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File($"/logs/analyzer.log", rollingInterval: RollingInterval.Day)
                .WriteTo.Console()
                .CreateLogger();

            // Setup DI
            var services = new ServiceCollection();
            services.AddLogging(c => c.AddSerilog());            
            services.AddNodeServices(options => { }); // TODO remove and just use the ML Model!
            services.AddSingleton<IRabbitEndpoint, RabbitEndpoint>();
            services.AddSingleton<IPipeline, RabbitClient>();
            services.AddSingleton<IEngine, Engine>();
            services.AddSingleton<IMediator, Mediator>();
            services.AddSingleton<IMLModel, MLModel>();
            services.AddSingleton<IAFINN, AFINN>();
            

            
            var serviceProvider = services.BuildServiceProvider();
            _rabbitEndpoint = serviceProvider.GetRequiredService<IRabbitEndpoint>();
            _pipeline = serviceProvider.GetRequiredService<IPipeline>();
            var engine = serviceProvider.GetRequiredService<IEngine>();
            engine.Init();

            // Handle cancel press and SIGTERM 
            AssemblyLoadContext.Default.Unloading += SigTermEventHandler;
            Console.CancelKeyPress += CancelHandler;

            _rabbitEndpoint.StartListening();
            _pipeline.Open();

        }

        private static void SigTermEventHandler(AssemblyLoadContext obj)
        {
            Log.Information("Unloading...");
            _rabbitEndpoint.StopListening();
            _pipeline.Close();
        }
        private static void CancelHandler(object sender, ConsoleCancelEventArgs e)
        {
            Log.Information("Exiting...");
            _rabbitEndpoint.StopListening();
            _pipeline.Close();
        }
    }
}
