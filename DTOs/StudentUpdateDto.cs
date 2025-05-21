using IdentityServer4.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentEmplacementApp.DTOs
{
    public class StudentProfileUpdateDto
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? FatherName { get; set; }

        [StringLength(7)]
        public string? Pin { get; set; }

        [MinLength(6)]
        public string? Password { get; set; }

        [MaxLength(15, ErrorMessage = "You can code maximum 15 majors")]
        public List<int>? Codes { get; set; }
    }
}
