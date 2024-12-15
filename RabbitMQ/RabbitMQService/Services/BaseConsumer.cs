using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQService.Services
{
    public abstract class BaseConsumer<T>(string service, string exchange, string routingKey) : BackgroundService
    {
        private readonly string _queueName = $"{service}_{exchange}_{routingKey}";


        // Method to start consuming messages
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };
            using var connection = await factory.CreateConnectionAsync(cancellationToken);
            using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
            // Declare the queue
            await channel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, args) =>
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    T model = DeserializeMessage(message);
                    await ProcessMessageAsync(model);
                    await channel.BasicAckAsync(args.DeliveryTag, false);
                }
                catch
                {
                    await channel.BasicNackAsync(args.DeliveryTag, false, true);
                }
            };

            await channel.BasicConsumeAsync(queue: _queueName,
                                   autoAck: false,
                                   consumer: consumer);

            await Task.Delay(-1, cancellationToken);

        }


        protected abstract T DeserializeMessage(string message);

        protected abstract Task ProcessMessageAsync(T model);
    }
}
