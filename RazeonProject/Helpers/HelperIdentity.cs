using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace RazeonProject.Helpers
{
    public class HelperIdentity
    {
        private static readonly byte[] SecretKey = Encoding.ASCII.GetBytes("LLAVESECRETARAZEONPROJECT123456789");
        private static readonly TimeSpan LifeTime = TimeSpan.FromMinutes(30);
        private static readonly JwtSecurityTokenHandler tokenHandler;
        private static readonly TokenValidationParameters validationParameters;

        static HelperIdentity()
        {
            tokenHandler = new JwtSecurityTokenHandler();
            validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(SecretKey),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };
        }

        public static string GenerateToken(string email, string user_type)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.Add(LifeTime),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, email),                  
                    new Claim("user_type", user_type),
                    new Claim(ClaimTypes.Expiration, DateTime.UtcNow.Add(LifeTime).ToString())
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(SecretKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static ClaimsPrincipal ValidateToken(string token)
        {
            SecurityToken validatedToken;

            return tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
        }
    }
}

