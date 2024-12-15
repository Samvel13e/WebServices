using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Api.Model;
using NotificationService.Api.Services.IServices;

namespace NotificationService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController(INotification notifactionService) : ControllerBase
    {
        private readonly INotification _notifactionService = notifactionService;

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("SendNotificatin")]
        public async Task<IActionResult> SendNotificatinAsync([FromBody] NotificationRequest request)
        {
            await _notifactionService.SendNotificatinAsync(request);

            return Ok("Notificatin sent successfully!");
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetNotificatinsAsync([FromQuery] NotificationFilter filter)
        {
            return Ok(await _notifactionService.GetNotificatinsAsync(filter));
        }
    }
}
