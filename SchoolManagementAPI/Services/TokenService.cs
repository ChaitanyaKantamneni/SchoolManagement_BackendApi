using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static SchoolManagementAPI.DAL.SchoolManagementDAL;

namespace SchoolManagementAPI.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config) { _config = config; }

        public (string accessToken, string refreshToken, DateTime accessExpiry, DateTime refreshExpiry) GenerateTokens(
            string email,
            string fullName,
            string role,
            string? schoolID = null) // optional SchoolID
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var nowIST = DateTimeHelper.NowIST();

            var accessExpiry = nowIST.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiryMinutes"]));
            var refreshExpiry = nowIST.AddDays(30);

            
            var claims = new List<Claim>
            {
                new Claim("email", email),    
                new Claim("name", fullName),
                new Claim("role", role)        
            };

            if (!string.IsNullOrEmpty(schoolID) && role != "1")
            {
                claims.Add(new Claim("SchoolID", schoolID));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var accessTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = accessExpiry,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var accessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(accessTokenDescriptor));
            var refreshToken = Guid.NewGuid().ToString("N");

            return (accessToken, refreshToken, accessExpiry, refreshExpiry);
        }
    }
}
