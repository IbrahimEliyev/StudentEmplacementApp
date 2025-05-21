namespace StudentEmplacementApp.DTOs
{
    public class ChoiceDto
    {
        public UniversityDto University { get; set; }
        public MajorDto Major { get; set; }
        public float? EnterenceScore { get; set; }
        public int NumOfPlaces { get; set; }
        public int Code { get; private set; }

        // create another Dto that includes this property (if it is possible do not show choices of student)  
        // public List<StudentDto> Students { get; set; } = new List<StudentDto>();
        
    }
}
