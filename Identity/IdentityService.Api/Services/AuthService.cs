using IdentityService.Api.Data;
using IdentityService.Api.Models;
using IdentityService.Api.Models.Dto;
using IdentityService.Api.Services.IServices;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Api.Service
{
    public sealed class AuthService(IdentityServiceDbContext db,
                                    UserManager<ApplicationUser> userManager,
                                    RoleManager<IdentityRole> roleManager,
                                    IJwtTokenGenerator jwtTokenGenerator) : IAuthService
    {
        private readonly IdentityServiceDbContext _db = db;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                bool existRole = await _roleManager.RoleExistsAsync(roleName);
                if (!existRole)
                {
                    // create role if does not exsist
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto) // TODO check order of check and null
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == loginRequestDto.UserName);

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (user == null || !isValid)
            {
                return new LoginResponseDto { Token = string.Empty }; // TODO check response Token = string.Empty
            }
            var roles = await _userManager.GetRolesAsync(user);
            string token = _jwtTokenGenerator.GenerateToken(user, roles);

            UserDto userDto = new()
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };
            LoginResponseDto loginResponseDto = new()
            {
                User = userDto,
                Token = token
            };
            return loginResponseDto;
        }

        public async Task<bool> IsEmailAlreadyRegistered(string email)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto) // TODO add nullcheck
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    //var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registrationRequestDto.Email);
                    //UserDto userDto = new() // TODO : check need or not
                    //{
                    //    Email = userToReturn.Email,
                    //    ID = userToReturn.Id,
                    //    Name = userToReturn.Name,
                    //    PhoneNumber = userToReturn.PhoneNumber
                    //};
                    return string.Empty;
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception) // TODO should to change
            {
            }
            return "Error  Encountered";
        }
    }
}
