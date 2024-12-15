using CommonService.Enum;

namespace NotificationService.Api.Model
{
    public class NotificationRequest
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
