using IdentityServer4.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentEmplacementApp.DTOs
{
    public class StudentScoreUpdateDto
    {
        [Required]
        [Range(0, 700, ErrorMessage = "You are out of bounds")]
        public float? Score { get; set; }

        [Required]
        [Range(0, 300, ErrorMessage = "Your secondary score should be in [0,300] range")]
        public float? SecondaryScore { get; set; }
    }
}