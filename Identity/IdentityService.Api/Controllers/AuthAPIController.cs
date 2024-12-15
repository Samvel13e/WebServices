using CommonService.Enum;
using IdentityService.Api.Models.Dto;
using IdentityService.Api.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using RabbitMQService.Constants;
using RabbitMQService.Models;
using RabbitMQService.Services.IServices;

namespace IdentityService.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController(IAuthService authService,
                                   IRabbitMqPublisher publisher) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly IRabbitMqPublisher _publisher = publisher;
        private readonly ResponseDto _response = new();

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            string errorMessage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new Exception(errorMessage);
            }
            var publishModel = new NotificationConsumerModel()
            {
                Channel = NotificationChannels.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                NotificationType=NotificationTypes.Registration,    
                Subject= "Congratulations",
                Body=$"Dear {model.Name}, you are successfully registered."
                
            };
            await _publisher.PublishAsync(ServiceNames.Identity, Exchanges.RegisterUserExchange, RoutingKeys.RegisterUserRoutingKey, publishModel);

            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            LoginResponseDto loginResponse = await _authService.Login(model);
            if (loginResponse.User == null)
            {
                throw new Exception("Username or password is incorrect");
            }
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            var assignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role.ToString().ToUpper());
            if (!assignRoleSuccessful)
            {
                throw new Exception("Error encountered");
            }
            return Ok(_response);
        }

        [HttpPost("IsEmailAlreadyRegistered")]
        public async Task<IActionResult> IsEmailAlreadyRegistered([FromBody] string email)
        {
            var isEmailFree = await _authService.IsEmailAlreadyRegistered(email);
            _response.Result = isEmailFree;
            return Ok(_response);
        }
    }
}
