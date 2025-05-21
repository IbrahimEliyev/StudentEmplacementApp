using StudentEmplacementApp.Models;

namespace StudentEmplacementApp.Interfaces
{
    public interface IChoiceRepository
    {
        public Task<Choice> GetChoiceByIdAsync(int uniId, int majorId);
        public Task<IEnumerable<Choice>> GetAllChoicesAsync();
        public Task<Choice> FindChoiceByCodeAsync(int code);
    }
}
