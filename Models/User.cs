using Abp.Authorization.Users;
using Microsoft.AspNetCore.Identity;

namespace StudentEmplacementApp.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        
        // Navigation property
        public Student? Student { get; set; }
    }   
}
