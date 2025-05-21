using System.ComponentModel.DataAnnotations;

namespace StudentEmplacementApp.DTOs
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "PIN is required.")]
        [RegularExpression(@"^[A-Za-z0-9]{7}$", ErrorMessage = "Personal identifier (PIN) must be 7 alphanumeric characters.")]
        public string Pin { get; set; }

        [Required(ErrorMessage = "PIN is required.")]
        [MinLength(6)]
        public string Password { get; set; }

    }
}