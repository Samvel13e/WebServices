using AutoMapper;
using CommonService.Enum;
using Microsoft.EntityFrameworkCore;
using NotificationService.Api.Data;
using NotificationService.Api.Model;
using NotificationService.Api.Services.IServices;
using RabbitMQService.Models;

namespace NotificationService.Api.Services
{
    public class Notification(IMailService mailService, ISmsService smsService, NotificationDbContext context, IMapper mapper) : INotification
    {
        private readonly IMailService _mailService = mailService;
        private readonly ISmsService _smsService = smsService;
        private readonly NotificationDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<List<NotificationHistory>> GetNotificatinsAsync(NotificationFilter filter)
        {
            var query = _context.NotificationHistories.AsQueryable();
            if (filter?.Channel != null)
            {
                query = query.Where(x => x.Channel == filter.Channel);
            }

            if (filter?.Status != null)
            {
                query = query.Where(x => x.Status == filter.Status);
            }

            if (filter?.Type != null)
            {
                query = query.Where(x => x.Type == filter.Type);
            }

            return await query.ToListAsync();
        }

        public async Task SendNotificatinAsync(NotificationRequest request)
        {
            switch (request.Channel)
            {
                case NotificationChannels.All:
                    {
                        await _mailService.SendEmailAsync(request);
                        await _smsService.SendMessageAsync(request);
                    }
                    break;
                case NotificationChannels.Sms:
                    {
                        await _smsService.SendMessageAsync(request);
                        break;
                    }
                case NotificationChannels.Email:
                    {
                        await _mailService.SendEmailAsync(request);
                        break;
                    }
                default:
                    throw new Exception("Invalid Notification Type");

            }
        }

        public async Task SendNotificatinAsync(NotificationConsumerModel request)
        {
            await SendNotificatinAsync(_mapper.Map<NotificationRequest>(request));
        }
    }
}
