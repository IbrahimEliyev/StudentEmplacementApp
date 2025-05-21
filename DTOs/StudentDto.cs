namespace StudentEmplacementApp.DTOs
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Score {  get; set; } = string.Empty;
        public string SecondaryScore {  get; set; } = string.Empty;
        public List<ChoiceDto> Choices { get; set; } = new List<ChoiceDto>();

        // Shown after Emplacement 
        public int? ResultCode { get; set; }
    }
}
