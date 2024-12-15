using Microsoft.AspNetCore.Mvc;
using NotificationService.Api.Model;
using RabbitMQService.Models;

namespace NotificationService.Api.Services.IServices
{
    public interface INotification
    {
        Task<List<NotificationHistory>> GetNotificatinsAsync(NotificationFilter filter);
        Task SendNotificatinAsync(NotificationRequest request);
        Task SendNotificatinAsync(NotificationConsumerModel request);
    }
}
