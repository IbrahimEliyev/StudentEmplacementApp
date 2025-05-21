using StudentEmplacementApp.Models;

namespace StudentEmplacementApp.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student> GetStudentByIdAsync(int id);
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<IEnumerable<Student>> GetOrderedStudentsAsync();
        Task AddStudentAsync(Student student);
        Task UpdateStudentAsync(Student student);
        Task DeleteStudentAsync(int id);
        public Task<Student> GetStudentByUserId(string userId);
        public Task SaveChangesAsync();

        // Write some specific methods if needed
    }
}
