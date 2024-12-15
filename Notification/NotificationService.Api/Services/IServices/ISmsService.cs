using NotificationService.Api.Model;

namespace NotificationService.Api.Services.IServices
{
    public interface ISmsService
    {
        Task SendMessageAsync(NotificationRequest request);
    }
}
