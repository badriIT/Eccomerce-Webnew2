using Eccomerce_Web.Common.Services.Interfaces;
using Eccomerce_Web.CORE;
using Eccomerce_Web.Models.Admin;
using Eccomerce_Web.Models.User;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Eccomerce_Web.Common.Services.implementations
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
            var credentials = new SigningCredentials(secuirityKey, SecurityAlgorithms.HmacSha256);


            var Claims = new[]
           {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
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
        public AdminToken GetAdminToken(Admin admin)
        {
            var jwtKey = "Blakhljkqrtojh134iotuoiewjytijdkljgaejktioqwejrwuokyhqoriejtoiwqtosdfsdfsdfsdfsdfsdfC";
            var jwtIssuer = "chven";
            var jwtAudience = "isini";
            var jwtDuration = 300;


            var secuirityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(secuirityKey, SecurityAlgorithms.HmacSha256);


            var Claims = new[]
           {
            new Claim(ClaimTypes.NameIdentifier, admin.id.ToString()),
            new Claim(ClaimTypes.Email, admin.email),
            new Claim(ClaimTypes.Role, admin.Role)
           };

            var token = new JwtSecurityToken(

                issuer: jwtIssuer,
                audience: jwtAudience,
                expires: DateTime.Now.AddMinutes(jwtDuration),
                claims: Claims,
                signingCredentials: credentials
            );

            return new AdminToken
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }





    }

}
