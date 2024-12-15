using NotificationService.Api.Model;

namespace NotificationService.Api.Services.IServices
{
    public interface IMailService
    {
        Task SendEmailAsync(NotificationRequest request);
    }
}
