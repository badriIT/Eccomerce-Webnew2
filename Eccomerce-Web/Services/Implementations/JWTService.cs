using Eccomerce_Web.CORE;
using Eccomerce_Web.Models;
using Eccomerce_Web.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Eccomerce_Web.Services.Implementations
{
    public class JWTService : IJWTService
    {
        public UserToken GetUserToken(UserProfile user)
        {
            var jwtKey = "Blakhljkqrtojh134iotuoiewjytijdkljgaejktioqwejrwuokyhqoriejtoiwqtosdfsdfsdfsdfsdfsdfC";
            var jwtIssuer = "chven";
            var jwtAudience = "isini";
            var jwtDuration = 300;

            

            var secuirityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));


            ;


            var credentials = new SigningCredentials(secuirityKey, SecurityAlgorithms.HmacSha256);

                ;

            var Claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            var token = new JwtSecurityToken(

                issuer: jwtIssuer,
                audience: jwtAudience,
                expires: DateTime.Now.AddMinutes(jwtDuration),
                claims: Claims,
                signingCredentials: credentials
            );

            return new UserToken
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }
       
    }

}
