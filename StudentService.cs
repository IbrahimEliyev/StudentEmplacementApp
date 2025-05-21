using StudentEmplacementApp.Interfaces;
using StudentEmplacementApp.Models;
using Microsoft.AspNetCore.Identity;

public class StudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IChoiceRepository _choiceRepository;
    private readonly UserManager<User> _userManager;

    public StudentService(IStudentRepository studentRepository, IChoiceRepository choiceRepository, UserManager<User> userManager)
    {
        _studentRepository = studentRepository;
        _choiceRepository = choiceRepository;
        _userManager = userManager;
    }
    
    public async Task AddStudentChoicesAsync(string userId, List<int> Codes)
    {
        var student = await _studentRepository.GetStudentByUserId(userId);

        foreach (var code in Codes)
        {
            var choice = await _choiceRepository.FindChoiceByCodeAsync(code);
            if (choice != null)
            {
                student.StudentChoices.Add(new StudentChoice
                {
                    Student = student,
                    Choice = choice,
                    UniId = choice.UniId,
                    MajorId = choice.MajorId
                });
            }
        }

        await _studentRepository.SaveChangesAsync();
    }

    public async Task<Student> GetStudentByPin(string pin)
    {
        var user = await _userManager.FindByNameAsync(pin);
        var student = await _studentRepository.GetStudentByUserId(user.Id);
        
        return student;
    }
}
