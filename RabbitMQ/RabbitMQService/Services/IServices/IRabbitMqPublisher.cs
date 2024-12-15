namespace RabbitMQService.Services.IServices
{
    public interface IRabbitMqPublisher
    {
        Task PublishAsync<T>(string queueName, string exchange, string routingKey, T message, CancellationToken cancellationToken = default);
    }
}
