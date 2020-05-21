using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using src.Domain.Ports.Outbound;
using storage.Domain.Models;

namespace src.Infrastructure
{
    public class RabbitClient : IPipeline
    {

        private IConnection connection;
        private IModel channel;

        private readonly ILogger _logger;
        public RabbitClient(ILogger<RabbitClient> logger)
        {
            _logger = logger;
        }
        public void SendToBot(AnalyzedSentence analyzedSentence)
        {
            _logger.LogInformation($"sending analyzed and stored sentence: {analyzedSentence.Sentence} to bot");
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            var body = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(analyzedSentence));
            channel.BasicPublish("", "bot_queue", properties, body);
        }
        public void Open()
        {
            var rabbitPassword = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS");
            _logger.LogInformation("connecting to rabbitmq");
            var factory = new ConnectionFactory() { HostName = "rabbit", UserName = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER"), Password = rabbitPassword };
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
            connection = TryOpenConnection(connection, factory);
            channel = connection.CreateModel();
            _logger.LogInformation("connected to rabbitmq");
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
        public void Close()
        {

            if (channel != null && connection != null)
            {
                _logger.LogInformation("closing connection to rabbitmq");
                channel.Close(200, "Goodbye");
                connection.Close();
            }
        }

    }
}