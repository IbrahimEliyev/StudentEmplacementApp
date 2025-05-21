using Microsoft.EntityFrameworkCore;
using StudentEmplacementApp.Data;
using StudentEmplacementApp.Interfaces;
using StudentEmplacementApp.Models;

namespace StudentEmplacementApp.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly DataContext _context;

        public StudentRepository(DataContext context)
        {
            _context = context;
        }

        public async Task AddStudentAsync(Student student)
        {
            await _context.AddAsync(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _context.Students
                .Include(s => s.StudentChoices)
                .ThenInclude(sc => sc.Choice)
                .ThenInclude(c => c.Major)
                .Include(s => s.StudentChoices)
                .ThenInclude(sc => sc.Choice)
                .ThenInclude(c => c.University)
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetOrderedStudentsAsync()
        {
            return await _context.Students.OrderByDescending(s => s.Score).ThenByDescending(s => s.SecondaryScore).Include(s => s.StudentChoices)
                .ThenInclude(sc => sc.Choice)
                .ThenInclude(c => c.Major)
                .Include(s => s.StudentChoices)
                .ThenInclude(sc => sc.Choice)
                .ThenInclude(c => c.University)
                .ToListAsync(); 
        }


        public async Task<Student> GetStudentByIdAsync(int id)
        {
            return await _context.Students.FindAsync(id);
        }
        public async Task<Student> GetStudentByUserId(string userId)
        {
            return await _context.Students
                 .Include(s => s.StudentChoices)
                 .ThenInclude(sc => sc.Choice)
                 .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        //public async Task<Student> GetStudentByPinAsync(string pin)
        //{

        //    var student = await _context.Students.FirstAsync();
        //    return student;
        //}


        public async Task UpdateStudentAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}