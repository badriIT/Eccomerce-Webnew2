using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Eccomerce_Web.Extensions;

public static class JwtExtensions
{
    public static IServiceCollection AddAppAuthentication(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = "chven",
                    ValidAudience = "isini",

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            "Blakhljkqrtojh134iotuoiewjytijdkljgaejktioqwejrwuokyhqoriejtoiwqtosdfsdfsdfsdfsdfsdfC"
                        )
                    ),

                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }
}