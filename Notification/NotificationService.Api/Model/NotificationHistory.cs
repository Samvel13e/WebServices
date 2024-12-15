using CommonService.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotificationService.Api.Model
{
    public class NotificationHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Message { get; set; }
        [MaxLength(200)]
        public string Subject { get; set; }
        [MaxLength(200)]
        public string SendTo { get; set; }
        public EmailSendingStatuses Status { get; set; }
        public DateTime CreatedTime { get; set; }
        public NotificationTypes Type { get; set; }
        public NotificationChannels Channel { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
