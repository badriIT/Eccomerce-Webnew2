using System.Text.Json.Serialization;
using Eccomerce_Web.Common.Services.implementations;
using Eccomerce_Web.Common.Services.Interfaces;
using Eccomerce_Web.Common.Services.ServiceModels;

namespace Eccomerce_Web.Extensions;

public static class serviceExtensions
{
    public static IServiceCollection AddAppControllers(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters
                    .Add(new JsonStringEnumConverter());
            });

        return services;
    }

    public static IServiceCollection AddCommonServices(this IServiceCollection services, ConfigurationManager configuration)

    {
        services.Configure<EmailSettings>(
        configuration.GetSection("EmailSettings"));

        services.AddScoped<IJWTService, JWTService>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IPhoneSender, PhoneSender>();

        return services;
    }
}