using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Resort_Management_System_API.Models;

namespace Resort_Management_System_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        #region Configuration Fields 
        private readonly ResortManagementContext context;
        private readonly IConfiguration configuration;

        public LoginController(ResortManagementContext _context, IConfiguration _configuration)
        {
            context = _context;
            configuration = _configuration;
        }
        #endregion

        #region Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login loginUser)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.UserName == loginUser.UserName && u.Password == loginUser.Password);

            if (user == null)
                return Unauthorized(new { message = "Invalid username or password" });

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token,
                user = new { user.UserId, user.UserName}
            });
        }
        #endregion

    
        // 🔑 Generate Token with Role & Expiry from appsettings.json
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var expiryMinutes = Convert.ToDouble(jwtSettings["TokenExpiryMinutes"]);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
