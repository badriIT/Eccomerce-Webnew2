using System.Threading.RateLimiting;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.RateLimiting;

namespace Eccomerce_Web.Common.Extensions
{
    public static class RateLimiter
    {
        public static IServiceCollection AddRateLimiter(this IServiceCollection services)
        {

    

            services.AddRateLimiter(options =>
            {
            
                options.AddPolicy("login", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 3,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));

                options.AddPolicy("register", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 2,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));

               
                options.AddPolicy("verify", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));

               
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 20,
                            Window = TimeSpan.FromSeconds(10),
                            QueueLimit = 0
                        }));

                options.RejectionStatusCode = 429;
            });

            return services;

        }
    }
}