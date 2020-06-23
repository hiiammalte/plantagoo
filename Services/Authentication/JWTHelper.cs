using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Plantagoo.Authentication
{
    public class JWTHelper : ITokenHelper
    {
        private readonly TokenSettings _tokenSettings;
        public JWTHelper(IOptions<TokenSettings> tokenSettings)
        {
            _tokenSettings = tokenSettings.Value;
        }
        public (string token, DateTime expiration) GenerateJWT(Guid userId, string userEmail)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Email, userEmail),
                }),
                Expires = DateTime.Now.AddMinutes(_tokenSettings.AccessExpirationInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Secret)), SecurityAlgorithms.HmacSha512Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return (tokenHandler.WriteToken(securityToken), (DateTime)tokenDescriptor.Expires);
        }
    }
}
