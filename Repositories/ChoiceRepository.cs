using Microsoft.EntityFrameworkCore;
using StudentEmplacementApp.Data;
using StudentEmplacementApp.Interfaces;
using StudentEmplacementApp.Models;

namespace StudentEmplacementApp.Repositories
{
    public class ChoiceRepository : IChoiceRepository
    {
        private readonly DataContext  _context;

        public ChoiceRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Choice> FindChoiceByCodeAsync(int code)
        {
            return await _context.Choices
                            .Include(c => c.Major)        
                            .Include(c => c.University)   
                          //  .Include(c => c.StudentChoices)
                            .FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<IEnumerable<Choice>> GetAllChoicesAsync()
        {
            return await _context.Choices
                            .Include(c => c.Major)        // Include Major entity
                            .Include(c => c.University)   // Include University entity
                       //     .Include(c => c.StudentChoices) // Include StudentChoices
                            .ToListAsync();
        }

        public async Task<Choice> GetChoiceByIdAsync(int uniId, int majorId)
        {
            return await _context.Choices.FindAsync(uniId , majorId);
        }
    }
}
