using CommonService.Enum;
using NotificationService.Api.Data;
using NotificationService.Api.Model;
using NotificationService.Api.Services.IServices;
using System.Net;
using System.Net.Mail;

namespace NotificationService.Api.Services
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;

        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPassword;
        private readonly IServiceProvider _serviceProvider;
        public MailService(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _smtpServer = _configuration["SMTP:Server"];
            _smtpPort = int.Parse(_configuration["SMTP:Port"]);
            _smtpUser = _configuration["SMTP:Username"];
            _smtpPassword = _configuration["SMTP:Password"];
        }


        public async Task SendEmailAsync(NotificationRequest request)
        {
            bool isSend = false;
            Exception error = null;
            if (request.Email != null && request.Body != null & request.Subject != null)
            {
                try
                {
                    var fromEmail = _smtpUser;
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(fromEmail),
                        Subject = request.Subject,
                        Body = request.Body,
                        IsBodyHtml = request.IsBodyHtml
                    };
                    mailMessage.To.Add(request.Email);

                    using var smtpClient = new SmtpClient(_smtpServer, _smtpPort);
                    smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                    smtpClient.EnableSsl = true;
                    await smtpClient.SendMailAsync(mailMessage);
                    isSend = true;

                }
                catch (Exception ex)
                {
                    isSend = false;
                    error = ex;
                }
            }
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
            var history = new NotificationHistory()
            {
                Channel = NotificationChannels.Email,
                CreatedTime = DateTime.Now,
                Message = request.Body,
                SendTo = request.Email,
                Status = isSend ? EmailSendingStatuses.Send : EmailSendingStatuses.Failed,
                Subject = request.Subject,
                Type = request.NotificationType,
                ErrorMessage = error?.ToString()
            };
            await context.NotificationHistories.AddAsync(history);
            await context.SaveChangesAsync();
        }
    }
}
