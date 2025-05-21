namespace StudentEmplacementApp.Models
{
    public class StudentChoice
    {
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        //Composite foreign key
        public int UniId { get; set; }
        public int MajorId { get; set; }
        public Choice? Choice { get; set; }

    }

}
