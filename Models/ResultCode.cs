using System.ComponentModel.DataAnnotations;

namespace StudentEmplacementApp.Models
{
    public class ResultCode
    {
        [Key]
        public int? ChoiceCode { get; set; }

        public ICollection<Student> Students { get; set; }
        public Choice? Choice { get; set; }

    }
}
