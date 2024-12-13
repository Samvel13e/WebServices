using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IdentityService.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(200)]
        public string Name { get; set; }       
    }
}
