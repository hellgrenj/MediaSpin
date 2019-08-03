using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using storage.Domain.Commands;
using storage.Domain.Models;
using storage.Domain.Ports.Inbound;

namespace storage.Infrastructure
{
    public class RabbitEndpoint : BackgroundService
    {
        private IConnection connection;
        private IModel channel;
        private readonly ILogger<RabbitEndpoint> _logger;

        public IServiceScopeFactory _serviceScopeFactory;
        public RabbitEndpoint(ILogger<RabbitEndpoint> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }
        private void StartListening()
        {
             var rabbitPassword = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS");
            _logger.LogInformation("connecting to rabbitmq");
            var factory = new ConnectionFactory() { HostName = "rabbit", UserName = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER"), Password = rabbitPassword };
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
            connection = TryOpenConnection(connection, factory);
            channel = connection.CreateModel();

            var queueName = channel.QueueDeclare("storage_queue", durable: true, exclusive: false,
                                 autoDelete: false,
                                 arguments: null).QueueName;
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            _logger.LogInformation("waiting for messages");
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var json = Encoding.UTF8.GetString(body);

                try
                {
                    HandleMessage(json).Wait();
                }
                catch (Exception e)
                {
                    _logger.LogError($"Failed to process message with exception {e.Message} stacktrace {e.StackTrace}");
                }
                finally
                {
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }

            };

            channel.BasicConsume(queue: queueName,
                                 autoAck: false,
                                 consumer: consumer);
        }

        private async Task HandleMessage(string json)
        {
            _logger.LogInformation("Received a message");
            var analyzedSentence = JsonConvert.DeserializeObject<AnalyzedSentence>(json);
            // Each message in its own scope for ef core
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.SendAsync(new StoreAnalyzedSentenceCommand() { AnalyzedSentence = analyzedSentence });
            }
        }
        private IConnection TryOpenConnection(IConnection connection, ConnectionFactory factory, int attempt = 0)
        {

            try
            {
                return connection = factory.CreateConnection();
            }
            catch (Exception e)
            {
                attempt++;
                if (attempt < 10)
                {
                    _logger.LogWarning($"Failed to connect to RabbitMQ attempt {attempt} retrying in 5 seconds");
                    System.Threading.Thread.Sleep(5000);
                    return TryOpenConnection(connection, factory, attempt);
                }
                else
                {
                    _logger.LogError($"Failed to connect after 10 attemps, exception {e.ToString()}");
                    throw;
                }
            }
        }
        private void StopListening()
        {

            if (channel != null && connection != null)
            {
                _logger.LogInformation("closing connection to rabbitmq");
                channel.Close(200, "Goodbye");
                connection.Close();
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            StartListening();
            return Task.CompletedTask;
        }
        public override void Dispose()
        {
            StopListening();
            base.Dispose();
        }
    }
}