using Microsoft.AspNetCore.Identity;  
using Microsoft.IdentityModel.Tokens;
using StudentEmplacementApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentEmplacementApp
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;

        public TokenService(IConfiguration config, UserManager<User> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<string> GenerateToken(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            // Create claims
            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.NameIdentifier, user.Id),
               new Claim(ClaimTypes.Name, user.UserName)
            };



            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Create the JWT token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}