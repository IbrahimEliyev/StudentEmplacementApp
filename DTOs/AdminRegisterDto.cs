using System.ComponentModel.DataAnnotations;

namespace StudentEmplacementApp.DTOs
{
    public class AdminRegisterDto
    {
        [Required]
        [StringLength(7)]
        public string Pin { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

    }
}
