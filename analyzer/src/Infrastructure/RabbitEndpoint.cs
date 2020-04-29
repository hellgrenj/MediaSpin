using System;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using src.Domain.Commands;
using src.Domain.Models;
using src.Domain.Ports.Inbound;

namespace src.Infrastructure
{
    public class RabbitEndpoint : IRabbitEndpoint
    {
        private IConnection connection;
        private IModel channel;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public RabbitEndpoint(ILogger<RabbitEndpoint> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
        public void StartListening()
        {
            var rabbitPassword = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS");
            _logger.LogInformation("connecting to rabbitmq");
            var factory = new ConnectionFactory() { HostName = "rabbit", UserName = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER"), Password = rabbitPassword };
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
            connection = TryOpenConnection(connection, factory);
            channel = connection.CreateModel();
            var queueName = channel.QueueDeclare("analytics_queue", durable: true, exclusive: false,
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
                    HandleMessage(json);
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

        private void HandleMessage(string json)
        {
            _logger.LogInformation("Received a message");
            var article = JsonConvert.DeserializeObject<Article>(json);
            _mediator.Send(new AnalyzeArticleCommand() { Article = article });
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
        public void StopListening()
        {

            if (channel != null && connection != null)
            {
                _logger.LogInformation("closing connection to rabbitmq");
                channel.Close(200, "Goodbye");
                connection.Close();
            }
        }

        public bool IsConnectionsOpen()
        {
            return connection.IsOpen && channel.IsOpen;
        }
    }
}