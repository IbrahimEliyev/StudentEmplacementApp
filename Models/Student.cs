using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StudentEmplacementApp.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FatherName {  get; set; } = string.Empty;
        public float Score { get; set; }
        public float SecondaryScore { get; set; }

        public int? ResultCode { get; set; }

        // A property : list of majors (codes:int) ,it can be another table that have one to many relationship with student 
        public ICollection<StudentChoice> StudentChoices { get; set; } = new List<StudentChoice>();

        // Authentication - related fields : not migrated (applied) yet
        public User? User { get; set; }
        public string UserId { get; set; }

        

        //  public Choice? Choice { get; set; }

        //  public List<int> Codes { get; set; } = new List<int>();
    }
}   