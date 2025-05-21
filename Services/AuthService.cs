using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TeamTaskManagement.API.Interfaces;
using TeamTaskManagement.API.Models;
using TeamTaskManagement.API.Response;
using TeamTaskManagementAPI.Data;

namespace TeamTaskManagement.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<BaseResponse<string>> RegisterAsync(RegisterDto dto)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                    return new BaseResponse<string>
                    {
                        Data = "Email already registered.",
                        Message = "Failed",
                        ResponseCode = ResponseCodes.FAILURE
                    };

                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return new BaseResponse<string>
                {
                    Data = GenerateJwt(user),
                    Message = "Success",
                    ResponseCode = ResponseCodes.SUCCESS
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<string>
                {
                    Data = ex.Message,
                    Message = "An error occurred while creating user profile... Please contact support",
                    ResponseCode = ResponseCodes.FAILURE
                };
            }

        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new Exception("Invalid credentials.");

            return GenerateJwt(user);
        }

        public async Task<UserDto> GetCurrentUserAsync(string userId)
        {
            var user = await _context.Users.FindAsync(Guid.Parse(userId));
            return new UserDto { Id = user.Id, Username = user.Username, Email = user.Email };
        }

        private string GenerateJwt(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}