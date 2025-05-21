using System.ComponentModel.DataAnnotations;

namespace StudentEmplacementApp.DTOs
{
    public class CodeDto
    {
        [Required]
        [MaxLength(15, ErrorMessage = "You can code maximum 15 majors")]
        public List<int> Codes { get; set; }
    }
}
