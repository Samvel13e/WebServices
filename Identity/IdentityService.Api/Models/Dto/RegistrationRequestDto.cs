using IdentityService.Api.Constants;
using System.ComponentModel.DataAnnotations;

namespace IdentityService.Api.Models.Dto
{
    public sealed class RegistrationRequestDto
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public Roles Role { get; set; }
    }
}
