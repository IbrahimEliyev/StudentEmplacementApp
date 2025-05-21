namespace StudentEmplacementApp.Models
{
    public class University
    {
        public int Id { get; set; }
        public string ShortName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public ICollection<Choice> Choices { get; set; } = new List<Choice>();

    }
}
