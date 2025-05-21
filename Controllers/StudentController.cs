using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentEmplacementApp.DTOs;
using StudentEmplacementApp.Helper;
using StudentEmplacementApp.Interfaces;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace StudentEmplacementApp.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;
        private readonly EmplacementService _emplacementService;
        private readonly StudentService _studentService;
        private readonly ILogger<StudentController> _logger;

        public StudentController(EmplacementService emplacementService, StudentService studentService,
                                 IStudentRepository studentRepository, IMapper mapper, ILogger<StudentController> logger)
        {
            _studentRepository = studentRepository;
            _mapper = mapper;
            _emplacementService = emplacementService;
            _studentService = studentService;
            _logger = logger;
        }
        /// <summary>
        /// Retrieves a student by their personal identifier (PIN).
        /// </summary>
        /// <param name="pin">The personal identifier (PIN) of the student (7 alphanumeric characters).</param>
        /// <returns>A student object if found; otherwise, a 404 Not Found response.</returns>
        /// <response code="200">Returns the student details.</response>
        /// <response code="400">If the PIN is null, empty, or invalid.</response>
        /// <response code="403">If the user is not authorized to access the student's data.</response>
        /// <response code="404">If no student is found with the specified PIN.</response>
        /// <response code="500">If an unexpected error occurs.</response>
        [Authorize]
        [HttpGet("{pin}")]
        public async Task<IActionResult> GetStudentAsync(string pin)
        {
            if (string.IsNullOrWhiteSpace(pin))
            {
                _logger.LogWarning("PIN is null or empty for GetStudent request.");
                return BadRequest("PIN cannot be null or empty.");
            }

            if (!Regex.IsMatch(pin, @"^[A-Za-z0-9]{7}$"))
            {
                _logger.LogWarning("Invalid PIN format: {Pin}", pin);
                return BadRequest("Length of PIN should be 7.");
            }

            var userPin = User.FindFirst("Pin")?.Value;
            var isAdmin = User.IsInRole("Admin");
            if (userPin != pin && !isAdmin)
            {
                _logger.LogWarning("User {UserId} unauthorized to access student with PIN {Pin}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value, pin);
                return Forbid("You are not authorized to access this student's data.");
            }

            try
            {
                var student = await _studentService.GetStudentByPin(pin);
                if (student == null)
                {
                    _logger.LogWarning("Student with PIN {Pin} not found.", pin);
                    return NotFound("Student not found.");
                }

                var studentDto = _mapper.Map<StudentDto>(student);
                return Ok(studentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student with PIN {Pin}", pin);
                return StatusCode(500, "An error occurred while retrieving the student.");
            }
        }


        /// <summary>
        /// Retrieves all students.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudentsAsync()
        {
            var students = await _studentRepository.GetAllStudentsAsync();
            var studentDtos = _mapper.Map<IEnumerable<StudentDto>>(students);

            return Ok(studentDtos);
        }

        /// <summary>
        /// Retrieves all students ordered by their scores
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("ordered")]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetOrderedStudentsAsync()
        {
            var orderedStudents = await _studentRepository.GetOrderedStudentsAsync();
            var orderedStudentDtos = _mapper.Map<IEnumerable<StudentDto>>(orderedStudents);

            return Ok(orderedStudentDtos);
        }

        /// <summary>
        /// Retrieves students with emplacement results.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("emplaced")]
        public async Task<IActionResult> GetStudentsWithEmplacementResults()
        {
            // Get students first, await for the result
            var students = await _studentRepository.GetAllStudentsAsync();
            await _emplacementService.EmplaceStudentAsync();
            var studentDtos = _mapper.Map<IEnumerable<StudentDto>>(students);

            // Perform emplacement logic afterwards, ensuring no concurrent access to DbContext

            // Return results after emplacement
            return Ok(studentDtos);
        }

        /// <summary>
        /// Adds list of student choices.
        /// </summary>
        /// <param name="dto">The data containing the list of choice codes.</param>
        /// <returns>A confirmation message if successful.</returns>
        /// <response code="200">If the student choices are added or updated successfully.</response>
        /// <response code="400">If the provided data is invalid.</response>
        /// <response code="401">If the user is not authenticated or the user ID is missing.</response>
        /// <response code="403">If the user is not in the Student role.</response>
        /// <response code="500">If an unexpected error occurs.</response>
        [Authorize(Roles = "Student")]
        [HttpPost("code")]
        public async Task<IActionResult> CodeChoices([FromBody]CodeDto dto)
        {
            if (dto == null || dto.Codes == null)
            {
                _logger.LogWarning("Invalid CodeDto provided for CodeChoices request.");
                return BadRequest("Codes list is required.");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                _logger.LogWarning("Missing user ID claim in JWT token for CodeChoices request.");
                return Unauthorized("User ID claim is missing from the token.");
            }

            try
            {
                await _studentService.AddStudentChoicesAsync(userId, dto.Codes);
                _logger.LogInformation("Student choices added for user ID {UserId}", userId);
                return Ok("Choices added successfully.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request for user ID: {UserId}", userId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding student choices for user ID: {UserId}", userId);
                return StatusCode(500, "An error occurred while adding student choices. Please try again later.");
            }

        }
        /// <summary>
        /// Partially updates the authenticated student's profile (first name, last name, father name, PIN, password).
        /// </summary>
        /// <param name="dto">The student profile data to update (fields are optional, scores cannot be updated).</param>
        /// <returns>A confirmation message if successful.</returns>
        /// <response code="200">If the student profile is updated successfully.</response>
        /// <response code="400">If the provided data is invalid or missing.</response>
        /// <response code="401">If the user is not authenticated or the user ID is missing.</response>
        /// <response code="403">If the user is not in the Student role.</response>
        /// <response code="404">If the student is not found.</response>
        /// <response code="500">If an unexpected error occurs.</response>
        [Authorize(Roles = "Student")]
        [HttpPatch("profile")]
        public async Task<IActionResult> UpdateStudentProfile([FromBody] StudentProfileUpdateDto dto)
        {
            // Validate input
            if (dto == null)
            {
                return BadRequest("Update data is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get user ID from nameidentifier claim
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Processing UpdateStudentProfile for user ID: {UserId}", userId);

            if (userId == null)
            {
                _logger.LogWarning("Missing user ID claim in JWT token.");
                return Unauthorized("User ID claim is missing from the token.");
            }

            try
            {
                await _studentService.UpdateStudentProfileAsync(userId, dto);
                return Ok("Student profile updated successfully.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request for user ID: {UserId}", userId);
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Failed to update profile for user ID: {UserId}", userId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student profile for user ID: {UserId}", userId);
                return StatusCode(500, "An error occurred while updating student profile. Please try again later.");
            }
        }


        /// <summary>
        /// Partially updates the authenticated student's score(s) (score and secondary score).
        /// </summary>
        /// <param name="studentId">The ID of the student to update (must be > 0).</param>
        /// <param name="dto">The score data to update (at least one field must be provided).</param>
        /// <returns>A confirmation message if successful.</returns>
        /// <response code="200">If the student scores are updated successfully.</response>
        /// <response code="400">If the student ID is invalid, the provided data is invalid, or no score fields are provided.</response>
        /// <response code="403">If the user is not in the Admin role.</response>
        /// <response code="404">If the student is not found.</response>
        /// <response code="500">If an unexpected error occurs.</response>
        [Authorize(Roles = "Admin")]
        [HttpPatch("{studentId}/scores")]
        public async Task<IActionResult> UpdateStudentScores(int studentId, [FromBody] StudentScoreUpdateDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Score data is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ensure at least one field is provided
            if (!dto.Score.HasValue && !dto.SecondaryScore.HasValue)
            {
                return BadRequest("At least one score field (Score or SecondaryScore) must be provided.");
            }

            try
            {
                await _studentService.UpdateStudentScoresAsync(studentId, dto);
                return Ok("Student scores updated successfully.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request for student ID: {StudentId}", studentId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student scores for student ID: {StudentId}", studentId);
                return StatusCode(500, "An error occurred while updating student scores. Please try again later.");
            }
        }

        /// <summary>
        /// Deletes a student by their personal identifier (PIN).
        /// </summary>
        /// <param name="pin">The personal identifier (PIN) of the student to delete (7 alphanumeric characters).</param>
        /// <returns>A confirmation message if successful.</returns>
        /// <response code="200">If the student is deleted successfully.</response>
        /// <response code="400">If the PIN is null, empty, or invalid.</response>
        /// <response code="403">If the user is neither an Admin nor the student with the matching PIN.</response>
        /// <response code="404">If the student is not found.</response>
        /// <response code="500">If an unexpected error occurs.</response>
        [Authorize]
        [HttpDelete("{pin}")]
        public async Task<IActionResult> DeleteStudent(string pin)
        {
            if (string.IsNullOrWhiteSpace(pin))
            {
                _logger.LogWarning("PIN is null or empty for GetStudent request.");
                return BadRequest("PIN cannot be null or empty.");
            }

            if (!Regex.IsMatch(pin, @"^[A-Za-z0-9]{7}$"))
            {
                _logger.LogWarning("Invalid PIN format: {Pin}", pin);
                return BadRequest("Length of PIN should be 7.");
            }

            var userPin = User.FindFirst("Pin")?.Value;
            var isAdmin = User.IsInRole("Admin");
            if (userPin != pin && !isAdmin)
            {
                _logger.LogWarning("User {UserId} unauthorized to access student with PIN {Pin}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value, pin);
                return Forbid("You are not authorized to access this student's data.");
            }

            try
            {
                await _studentService.DeleteStudentAsync(pin);
                _logger.LogInformation("Student ID {StudentId} deleted by super-admin {SuperAdminId}", pin, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return Ok("Student deleted successfully.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request to delete student ID: {StudentId}", pin);
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Failed to delete student ID: {StudentId}", pin);
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student ID: {StudentId}", pin);
                return StatusCode(500, "An error occurred while deleting the student. Please try again later.");
            }
        }

    }
}