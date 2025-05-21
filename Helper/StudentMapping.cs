using StudentEmplacementApp.DTOs;
using StudentEmplacementApp.Interfaces;
using StudentEmplacementApp.Models;

namespace StudentEmplacementApp.Helper
{
    public class StudentMapping
    {
        private readonly IChoiceRepository _choiceRepository;
        public StudentMapping(IChoiceRepository choiceRepository)
        {
            _choiceRepository = choiceRepository;
        }

        public async Task<Student> MapToStudent(StudentRegisterDto dto)
        {
            var student = new Student   
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                FatherName = dto.FatherName,
                Score = dto.Score,
                SecondaryScore = dto.SecondaryScore,
            };
                
            //foreach(var code in dto.Codes)
            //{
            //    var choice  = await _choiceRepository.FindChoiceByCodeAsync(code);
            //    if(choice != null)
            //    {
            //        student.StudentChoices.Add(new StudentChoice { 
            //            Student = student,
            //            Choice = choice,
            //            UniId = choice.UniId,
            //            MajorId = choice.MajorId
            //        });
            //    }
            //}

            return student;
        }
    }
}
