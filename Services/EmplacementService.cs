using StudentEmplacementApp.Interfaces;
using StudentEmplacementApp.Models;
using System.ComponentModel;

namespace StudentEmplacementApp
{
    public class EmplacementService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IChoiceRepository _choiceRepository;
        private readonly ILogger<EmplacementService> _logger;

        public EmplacementService(IStudentRepository studentRepository, IChoiceRepository choiceRepository, ILogger<EmplacementService> logger)
        {
            _studentRepository = studentRepository;
            _choiceRepository = choiceRepository;
            _logger = logger;
        }

        public async Task EmplaceStudentAsync()
        {
            var students = await _studentRepository.GetOrderedStudentsAsync();
            

            foreach (var student in students)
            {
                foreach (var studentChoice in student.StudentChoices)
                {
                    var choice = await _choiceRepository.FindChoiceByCodeAsync(studentChoice.Choice.Code);

                    if (choice is null)
                    {
                        _logger.LogWarning($"Choice with code {studentChoice.Choice.Code} not found");
                        continue;
                    }
                        

                    // Initialize availablePlaces with the value of choice.NumOfPlaces
                 //   int availablePlaces = choice.AvailablePlaces;

                    if (TryAssignStudentToChoice(choice, student))
                    {
                        _logger.LogInformation($"Student {student.Id} has been assigned succesefully to choice {choice.Code}");
                        break; // Student has been assigned, stop checking other choices
                    }

                }

                _studentRepository.UpdateStudentAsync(student);
            }
        }

        private bool TryAssignStudentToChoice( Choice choice, Student student)
        {
            if (choice.AvailablePlaces > 0)
            {
                // Add the student to the choice's StudentChoices
                choice.StudentChoices.Add(new StudentChoice
                {
                    StudentId = student.Id,
                    MajorId = choice.MajorId,
                    UniId = choice.UniId,
                    Student = student,
                    Choice = choice
                });

                // Set the result code of the student based on the choice
                student.ResultCode = choice.Code;
                choice.AvailablePlaces--;
                
                Console.WriteLine($"Assigned student {student.Id} to choice {choice.Code}. ResultCode: {student.ResultCode}, AvailablePlaces: {choice.AvailablePlaces}");

                return true; // Assignment successful
            }
            // If no more available places, set the entrance score for the choice
            
                choice.EnterenceScore = student.Score;
                _logger.LogInformation($"Choice {choice.Code} is full. Updated entrance score to {student.Score}.");
                return false;

        }
    }

}