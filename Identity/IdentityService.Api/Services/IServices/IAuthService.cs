using IdentityService.Api.Models.Dto;

namespace IdentityService.Api.Services.IServices
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(string email, string rolename);
        Task<bool> IsEmailAlreadyRegistered(string email);
    }
}
