using Abp.Authorization.Users;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentEmplacementApp.DTOs;
using StudentEmplacementApp.Helper;
using StudentEmplacementApp.Interfaces;
using StudentEmplacementApp.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentEmplacementApp.Controllers
{
    /// <summary>
    /// Manages user authentication operations, including registration and login for students and admins.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;
        private readonly StudentMapping _studentMapper;
        private readonly UserManager<User> _userManager;
        private readonly TokenService _tokenService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(
            IStudentRepository studentRepository,
            IMapper mapper,
            StudentMapping studentMapper,
            UserManager<User> userManager,
            TokenService tokenService,
            RoleManager<IdentityRole> roleManager)
        {
            _studentRepository = studentRepository;
            _mapper = mapper;
            _studentMapper = studentMapper;
            _userManager = userManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Registers a student.
        /// </summary>
        /// <param name="studentDto">The student registration data, including PIN, password, and other details.</param>
        /// <returns>A confirmation message and the student ID if successful.</returns>
        /// <response code="200">If the student is registered successfully, returns the student ID.</response>
        /// <response code="400">If the registration data is invalid, user creation fails, role creation fails, or role assignment fails.</response>
        /// <response code="500">If an unexpected error occurs during registration.</response>
        [HttpPost("register-student")]
        public async Task<ActionResult> RegisterStudent([FromBody] StudentRegisterDto studentDto)
        {
            if (studentDto == null)
            {
                return BadRequest(new { message = "Student registration data is required." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid registration data", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            var user = new User
            {
                UserName = studentDto.Pin,
            };

            var result = await _userManager.CreateAsync(user, studentDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "User creation failed", errors = result.Errors.Select(e => e.Description) });
            }

            var roleName = "Student";

            var roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                var createRoleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (!createRoleResult.Succeeded)
                {
                    return BadRequest(new { message = "Role creation failed", errors = createRoleResult.Errors.Select(e => e.Description) });
                }
            }

            var roleAssignmentResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!roleAssignmentResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return BadRequest(new { message = "Role assignment failed", errors = roleAssignmentResult.Errors.Select(e => e.Description) });
            }

            try
            {
                var student = await _studentMapper.MapToStudent(studentDto);
                student.UserId = user.Id; // Link the User to the Student

                await _studentRepository.AddStudentAsync(student);

                return Ok(new { message = "Student registered successfully", studentId = student.Id });
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(user);
                return StatusCode(500, new { message = "An error occurred while registering the student.", error = ex.Message });
            }
        }

        /// <summary>
        /// Registers an admin.
        /// </summary>
        /// <param name="adminDto">The admin registration data, including PIN, password, first name, and last name.</param>
        /// <returns>A confirmation message if successful.</returns>
        /// <response code="200">If the admin is registered successfully.</response>
        /// <response code="400">If the registration data is invalid, user creation fails, role creation fails, or role assignment fails.</response>
        /// <response code="500">If an unexpected error occurs during registration.</response>
        [HttpPost("register-admin")]
        public async Task<ActionResult> RegisterAdmin([FromBody] AdminRegisterDto adminDto)
        {
            if (adminDto == null)
            {
                return BadRequest(new { message = "Admin registration data is required." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid registration data", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            var user = new User
            {
                UserName = adminDto.Pin,
                FirstName = adminDto.FirstName,
                LastName = adminDto.LastName,
            };

            var result = await _userManager.CreateAsync(user, adminDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "User creation failed", errors = result.Errors.Select(e => e.Description) });
            }

            var roleName = "Admin";

            var roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                var createRoleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (!createRoleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    return BadRequest(new { message = "Role creation failed", errors = createRoleResult.Errors.Select(e => e.Description) });
                }
            }

            var roleAssignmentResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!roleAssignmentResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return BadRequest(new { message = "Role assignment failed", errors = roleAssignmentResult.Errors.Select(e => e.Description) });
            }

            return Ok(new { message = "Admin registered successfully" });
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token for accessing protected endpoints.
        /// </summary>
        /// <param name="dto">The login credentials, including PIN and password.</param>
        /// <returns>A JWT token if authentication is successful.</returns>
        /// <response code="200">If authentication is successful, returns the JWT token.</response>
        /// <response code="400">If the login data is invalid.</response>
        /// <response code="401">If the PIN or password is incorrect.</response>
        /// <response code="500">If an unexpected error occurs during authentication.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "Login credentials are required." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid login data", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            try
            {
                // Find user by username (Pin)
                var user = await _userManager.FindByNameAsync(dto.Pin);

                if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                {
                    return Unauthorized(new { message = "Invalid credentials." });
                }

                // Get the roles assigned to the user
                var roles = await _userManager.GetRolesAsync(user);

                // Create claims for the JWT token
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? "Student") // Default to Student if no role
                };

                // Generate the JWT token
                var token = await _tokenService.GenerateToken(user);

                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing login.", error = ex.Message });
            }
        }
    }
}