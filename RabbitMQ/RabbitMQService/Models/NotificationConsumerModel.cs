using CommonService.Enum;

namespace RabbitMQService.Models
{
    public class NotificationConsumerModel
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public NotificationChannels Channel { get; set; }
        public bool IsBodyHtml { get; set; }
        public NotificationTypes NotificationType { get; set; }
    }
}
