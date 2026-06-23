//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using static SchoolManagementAPI.DAL.SchoolManagementDAL;

//namespace SchoolManagementAPI.Services
//{
//    public class TokenService
//    {
//        private readonly IConfiguration _config;
//        public TokenService(IConfiguration config) { _config = config; }

//        public (string accessToken, string refreshToken, DateTime accessExpiry, DateTime refreshExpiry) GenerateTokens(
//            string email,
//            string fullName,
//            string role,
//            string? schoolID = null) // optional SchoolID
//        {
//            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
//            var nowIST = DateTimeHelper.NowIST();

//            var accessExpiry = nowIST.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiryMinutes"]));
//            var refreshExpiry = nowIST.AddDays(30);


//            var claims = new List<Claim>
//            {
//                new Claim("email", email),    
//                new Claim("name", fullName),
//                new Claim("role", role)        
//            };

//            if (!string.IsNullOrEmpty(schoolID) && role != "1")
//            {
//                claims.Add(new Claim("SchoolID", schoolID));
//            }

//            var tokenHandler = new JwtSecurityTokenHandler();
//            var accessTokenDescriptor = new SecurityTokenDescriptor
//            {
//                Subject = new ClaimsIdentity(claims),
//                Expires = accessExpiry,
//                Issuer = _config["Jwt:Issuer"],
//                Audience = _config["Jwt:Audience"],
//                SigningCredentials = new SigningCredentials(
//                    new SymmetricSecurityKey(key),
//                    SecurityAlgorithms.HmacSha256Signature)
//            };

//            var accessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(accessTokenDescriptor));
//            var refreshToken = Guid.NewGuid().ToString("N");

//            return (accessToken, refreshToken, accessExpiry, refreshExpiry);
//        }
//    }
//}

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SchoolManagementAPI.Services
{
    /// <summary>
    /// TokenService: Manages JWT token generation and validation.
    /// This service is used to enforce secure, stateless session authentication across the SchoolManagementERP.
    /// It generates HMAC-SHA256 encrypted Access tokens and UUID Refresh tokens.
    /// </summary>
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Generates a pair of JWT Access and Refresh tokens containing user claims.
        /// Access tokens are used in subsequent API requests. Refresh tokens are stored in the database to get new access keys.
        /// </summary>
        /// <param name="email">User's identity email</param>
        /// <param name="fullName">User's display name</param>
        /// <param name="role">Role ID (e.g. 1 = Super Admin, 2 = School Admin)</param>
        /// <param name="schoolId">Operational tenant context ID</param>
        /// <param name="schoolIds">List of authorized school identifiers</param>
        public (string AccessToken, string RefreshToken, DateTime AccessExpiryUtc, DateTime RefreshExpiryUtc)
            GenerateTokens(string email, string fullName, string role, string? schoolId = null, string? schoolIds = null)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var nowUtc = DateTime.UtcNow;

            var accessExpiryUtc = nowUtc.AddMinutes(
                Convert.ToDouble(_config["Jwt:ExpiryMinutes"])
            );

            var refreshExpiryUtc = nowUtc.AddDays(30);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Name, fullName),
                new Claim(ClaimTypes.Role, role)
            };

            if (!string.IsNullOrEmpty(schoolId) && role != "1")
            {
                claims.Add(new Claim("SchoolID", schoolId));
            }

            // NEW — group admin gets school list in token
            if (!string.IsNullOrEmpty(schoolIds) && role == "10") // 9 = group admin role ID
            {
                claims.Add(new Claim("SchoolIDs", schoolIds)); // comma-separated
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = nowUtc,
                Expires = accessExpiryUtc,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256
                )
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);

            return (
                handler.WriteToken(token),
                Guid.NewGuid().ToString("N"),
                accessExpiryUtc,
                refreshExpiryUtc
            );
        }
    }
}
