using CommonService.Enum;

namespace NotificationService.Api.Model
{
    public class NotificationFilter
    {
        public EmailSendingStatuses? Status { get; set; }
        public NotificationTypes? Type { get; set; }
        public NotificationChannels? Channel { get; set; }
    }
}
