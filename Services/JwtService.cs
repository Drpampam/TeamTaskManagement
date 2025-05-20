//using System;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;
//using TeamTaskManagement.API.Models;

//namespace TeamTaskManagement.API.Services
//{
//    public class JwtService
//    {
//        private readonly IConfiguration _configuration;

//        public JwtService(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public string GenerateToken(User user)
//        {
//            var tokenHandler = new JwtSecurityTokenHandler();
//            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
//            var tokenDescriptor = new SecurityTokenDescriptor
//            {
//                Subject = new ClaimsIdentity(new[]
//                {
//                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//                    new Claim(ClaimTypes.Email, user.Email),
//                    new Claim(ClaimTypes.GivenName, user.FirstName),
//                    new Claim(ClaimTypes.Surname, user.LastName)
//                }),
//                Expires = DateTime.UtcNow.AddDays(7),
//                SigningCredentials = new SigningCredentials(
//                    new SymmetricSecurityKey(key),
//                    SecurityAlgorithms.HmacSha256Signature)
//            };

//            var token = tokenHandler.CreateToken(tokenDescriptor);
//            return tokenHandler.WriteToken(token);
//        }
//    }
//} 