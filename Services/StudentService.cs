using StudentEmplacementApp.Interfaces;
using StudentEmplacementApp.Models;
using Microsoft.AspNetCore.Identity;
using StudentEmplacementApp.DTOs;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using StudentEmplacementApp.Data;

public class StudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IChoiceRepository _choiceRepository;
    private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;
    private readonly DataContext _context;
    public StudentService(IStudentRepository studentRepository, IChoiceRepository choiceRepository, Microsoft.AspNetCore.Identity.UserManager<User> userManager, DataContext context)
    {
        _studentRepository = studentRepository;
        _choiceRepository = choiceRepository;
        _userManager = userManager;
        _context = context;
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
        if (user == null)
        {
            return null;
        }

        // Find student by user ID
        var student = await _studentRepository.GetStudentByUserId(user.Id);
        if (student == null)
        {
            return null;
        }

        return student;
    }

    public async Task UpdateStudentProfileAsync(string userId, StudentProfileUpdateDto dto)
    {
        var student = await _studentRepository.GetStudentByUserId(userId);

        // Update User properties
        if (!string.IsNullOrEmpty(dto.FirstName))
        {
            student.FirstName = dto.FirstName;
        }
        if (!string.IsNullOrEmpty(dto.LastName))
        {
            student.LastName = dto.LastName;
        }
        if (!string.IsNullOrEmpty(dto.FatherName))
        {
            student .FatherName = dto.FatherName;
        }
        if (!string.IsNullOrEmpty(dto.Pin))
        {
            student.User.UserName = dto.Pin;
        }
        if (!string.IsNullOrEmpty(dto.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(student.User);
            var result = await _userManager.ResetPasswordAsync(student.User, token, dto.Password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Failed to update password: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }


    public async Task UpdateStudentScoresAsync(int studentId, StudentScoreUpdateDto dto)
    {
        // Find the Student
        var student = await _studentRepository.GetStudentByIdAsync(studentId);

        if (student == null)
        {
            throw new ArgumentException("Student not found.");
        }

        // Update scores
        if (dto.Score.HasValue)
        {
            student.Score = dto.Score.Value;
        }
        if (dto.SecondaryScore.HasValue)
        {
            student.SecondaryScore = dto.SecondaryScore.Value;
        }

        // Save changes only if at least one field was updated
        if (dto.Score.HasValue || dto.SecondaryScore.HasValue)
        {
            await _studentRepository.SaveChangesAsync();
        }
    }


    // Delete Student(Delete corresponding user so student)
    public async Task DeleteStudentAsync(string pin)
    {
        var student = await GetStudentByPin(pin);
        if (student == null)
        {
            throw new ArgumentException("Student not found.");
        }

        var user = await _userManager.FindByIdAsync(student.UserId);
        if (user == null)
        {
            throw new InvalidOperationException("Associated user not found.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Delete User (cascade will delete Student and StudentChoice)
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Failed to delete user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
