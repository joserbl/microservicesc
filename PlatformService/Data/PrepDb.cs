using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool IsProd)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>()!, IsProd);
            }
        }

        private static void SeedData(AppDbContext context, bool IsProd)
        {

            if(IsProd)
            {
                Console.WriteLine("INFO --> Trying to apply migrations...");
                try
                {
                    context.Database.Migrate();
                }
                catch(Exception ex)
                { 
                    Console.WriteLine($"ERR --> Coudn't apply migrations, {ex.Message}");
                }
            }

            if(context.Platforms.Any())
            {
                Console.WriteLine("INFO --> Data already exists");
                return;
            }
            Console.WriteLine("INFO --> Seeding some data HOLIS QUE ROLLIs");

            context.Platforms.AddRange(
                new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
                new Platform() { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
                new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" },
                new Platform() { Name = "PUCHIS", Publisher = "Pre Puchis Computing Foundation", Cost = "Free" }
            );
            context.SaveChanges();
        }
    }
}