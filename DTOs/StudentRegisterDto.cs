using System.ComponentModel.DataAnnotations;

namespace StudentEmplacementApp.DTOs
{
    public class StudentRegisterDto
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        public string? FatherName { get; set; }

        [Required]
        [Range(150,700 , ErrorMessage = "You are out of coding")]
        public float Score { get; set; }

        [Required]
        [Range(0, 300 , ErrorMessage = "Your secondary score should be in [0,300] range")]
        public float SecondaryScore { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z0-9]{7}$", ErrorMessage = "Personal identifier (PIN) must be 7 alphanumeric characters.")]
        public string Pin { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

    }
}
