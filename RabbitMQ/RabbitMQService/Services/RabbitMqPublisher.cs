using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQService.Services.IServices;
using System.Text;
using System.Text.Json;

namespace RabbitMQService.Services
{
    public class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly ConnectionFactory _factory;

        public RabbitMqPublisher()
        {
            _factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };
        }

        public async Task PublishAsync<T>(string service, string exchange, string routingKey, T message, CancellationToken cancellationToken = default)
        {
            using var connection = await _factory.CreateConnectionAsync(cancellationToken);
            using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
            var queueName = $"{service}_{exchange}_{routingKey}";

            await channel.ExchangeDeclareAsync(exchange, "direct", durable: true, cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
            await channel.QueueBindAsync(queueName, exchange, routingKey, cancellationToken: cancellationToken);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            // Publish the message
            await channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: routingKey,
                mandatory: false,
                body: body,
                cancellationToken: cancellationToken);
        }
    }
}
