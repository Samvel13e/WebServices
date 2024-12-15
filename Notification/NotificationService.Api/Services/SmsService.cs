using CommonService.Enum;
using NotificationService.Api.Data;
using NotificationService.Api.Model;
using NotificationService.Api.Services.IServices;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace NotificationService.Api.Services
{
    public class SmsService(IServiceProvider serviceProvider) : ISmsService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public async Task SendMessageAsync(NotificationRequest request)
        {
            bool isSend = false;
            Exception error = null;
            if (request.PhoneNumber != null && request.Body != null)
            {
                const string accountSid = "AC9009dceb1312edf2f01ac3f330ac2227";
                const string authToken = "7a579af5eab02a46f0b64549fc642847";
                const string fromNumber = "+14173522895";

                TwilioClient.Init(accountSid, authToken);
                try
                {
                    await MessageResource.CreateAsync(
                        body: request.Body,
                        from: new PhoneNumber(fromNumber),
                        to: new PhoneNumber(request.PhoneNumber)
                );
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
                Channel = NotificationChannels.Sms,
                CreatedTime = DateTime.Now,
                Message = request.Body,
                SendTo = request.PhoneNumber,
                Status = isSend ? EmailSendingStatuses.Send : EmailSendingStatuses.Failed,
                Subject = request.Subject,
                Type = request.NotificationType,
                ErrorMessage = error.ToString()
            };

            await context.NotificationHistories.AddAsync(history);

            await context.SaveChangesAsync();
        }
    }
}
