using Newtonsoft.Json;
using NotificationService.Api.Services.IServices;
using RabbitMQService.Constants;
using RabbitMQService.Models;
using RabbitMQService.Services;

namespace NotificationService.Api.Consumers
{
    public class RegisterConsumer(IServiceProvider serviceProvider)
        : BaseConsumer<NotificationConsumerModel>(ServiceNames.Identity, Exchanges.RegisterUserExchange, RoutingKeys.RegisterUserRoutingKey)
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        protected override NotificationConsumerModel DeserializeMessage(string message)
        {
            return JsonConvert.DeserializeObject<NotificationConsumerModel>(message);
        }


        protected override async Task ProcessMessageAsync(NotificationConsumerModel model)
        {
            using var scope = _serviceProvider.CreateScope();
            var _notification = scope.ServiceProvider.GetRequiredService<INotification>();

            await _notification.SendNotificatinAsync(model);

            await Task.CompletedTask;
        }

    }

}
