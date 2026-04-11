using System;
using Eccomerce_Web.Data;
using Microsoft.EntityFrameworkCore;

namespace Eccomerce_Web.Common.Extensions
{
    public static class SeedExtinsion
    {
        public static void Seed(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();

                context.Database.Migrate(); // IMPORTANT (auto applies migrations)
                DbSeeder.Seed(context);
            }
        }
    }
}
