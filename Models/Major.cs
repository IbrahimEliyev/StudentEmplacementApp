namespace StudentEmplacementApp.Models
{
    public class Major
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public InstructionLanguage Language { get; set; }
        public ICollection<Choice> Choices { get; set;} = new List<Choice>();
    }

}
