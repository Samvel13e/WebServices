using AutoMapper;
using NotificationService.Api.Model;
using RabbitMQService.Models;

namespace NotificationService.Api.Mappers
{
    public class MappingProfile : Profile
    {   
        public MappingProfile()
        {
            CreateMap<NotificationRequest, NotificationConsumerModel>().ReverseMap();
        }
    }
}
