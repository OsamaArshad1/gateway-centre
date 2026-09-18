using System.Security.Cryptography;
using GatewayCentre.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GatewayCentre.Web.Data;

public static class DbSeeder
{
    public const string AdministratorRole = "Administrator";

    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        var db = provider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();

        await SeedRolesAndAdminAsync(provider, configuration);
        await SeedSiteSettingsAsync(db);
        await SeedHallsAsync(db);
        await SeedPackagesAsync(db);
        await SeedTestimonialsAsync(db);

        await db.SaveChangesAsync();
    }

    private static async Task SeedRolesAndAdminAsync(IServiceProvider provider, IConfiguration configuration)
    {
        var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();

        if (!await roleManager.RoleExistsAsync(AdministratorRole))
        {
            await roleManager.CreateAsync(new IdentityRole(AdministratorRole));
        }

        var adminEmail = configuration["SeedAdmin:Email"] ?? "admin@gatewaycentre.pk";
        var configuredPassword = configuration["SeedAdmin:Password"];

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin is null)
        {
            var generatedPassword = string.IsNullOrWhiteSpace(configuredPassword) ? GenerateRandomPassword() : null;
            var adminPassword = generatedPassword ?? configuredPassword!;

            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, AdministratorRole);

                if (generatedPassword is not null)
                {
                    Console.WriteLine();
                    Console.WriteLine("==================================================================");
                    Console.WriteLine(" First run: an admin account was created with a generated password.");
                    Console.WriteLine($"   Email:    {adminEmail}");
                    Console.WriteLine($"   Password: {generatedPassword}");
                    Console.WriteLine(" Log in at /Account/Login and change this password immediately.");
                    Console.WriteLine(" To set a known password instead, configure SeedAdmin:Password");
                    Console.WriteLine(" (e.g. via the SeedAdmin__Password environment variable) before first run.");
                    Console.WriteLine("==================================================================");
                    Console.WriteLine();
                }
            }
        }
        else if (!await userManager.IsInRoleAsync(existingAdmin, AdministratorRole))
        {
            await userManager.AddToRoleAsync(existingAdmin, AdministratorRole);
        }
    }

    private static string GenerateRandomPassword()
    {
        const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lower = "abcdefghijkmnpqrstuvwxyz";
        const string digits = "23456789";
        const string special = "!@#$%^&*";
        const string all = upper + lower + digits + special;

        var chars = new char[16];
        chars[0] = upper[RandomNumberGenerator.GetInt32(upper.Length)];
        chars[1] = lower[RandomNumberGenerator.GetInt32(lower.Length)];
        chars[2] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
        chars[3] = special[RandomNumberGenerator.GetInt32(special.Length)];
        for (var i = 4; i < chars.Length; i++)
        {
            chars[i] = all[RandomNumberGenerator.GetInt32(all.Length)];
        }

        for (var i = chars.Length - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }

        return new string(chars);
    }

    private static async Task SeedSiteSettingsAsync(ApplicationDbContext db)
    {
        if (await db.SiteSettings.AnyAsync())
        {
            return;
        }

        db.SiteSettings.Add(new SiteSettings
        {
            SiteName = "Gateway Centre",
            Tagline = "Marriage Hall & Guest House",
            HeroHeadline = "Celebrate in Style at Gateway Centre",
            HeroSubheadline = "The perfect venue for your weddings, parties, and corporate events in Khanpur.",
            PrimaryPhone = "+92 303 6890241",
            SecondaryPhone = "+92 320 1889473",
            WhatsAppNumber = "+923201889473",
            Email = "info@gatewaycentre.pk",
            Address = "The Gateway Centre, Central Public School Road, Khanpur",
            Latitude = 28.6532915,
            Longitude = 70.6509576,
            FacebookUrl = "https://www.facebook.com/gateway.kpr/",
            InstagramUrl = "https://www.instagram.com/gateway.kpr",
            TwitterUrl = "https://x.com/gatewaykpr"
        });
    }

    private static async Task SeedHallsAsync(ApplicationDbContext db)
    {
        if (await db.Halls.AnyAsync())
        {
            return;
        }

        db.Halls.AddRange(
            new Hall
            {
                Name = "Ground Floor Hall",
                Floor = "Ground Floor",
                SeatingCapacity = 600,
                Description = "Our largest banquet hall, ideal for grand weddings, receptions, and large corporate gatherings. Spacious floor plan with flexible seating layouts and a dedicated stage area.",
                ImageUrl = "/images/hall-ground-floor.jpg",
                DisplayOrder = 1
            },
            new Hall
            {
                Name = "Second Floor Hall",
                Floor = "Second Floor",
                SeatingCapacity = 400,
                Description = "A more intimate hall, perfect for engagements, walimas, and formal celebrations, with elegant interiors and full event support.",
                ImageUrl = "/images/hall-second-floor.jpg",
                DisplayOrder = 2
            });
    }

    private static async Task SeedPackagesAsync(ApplicationDbContext db)
    {
        if (await db.Packages.AnyAsync())
        {
            return;
        }

        db.Packages.AddRange(
            new Package
            {
                Name = "Essential",
                Description = "Everything you need for a beautifully run event, without the extras.",
                StartingPriceRs = 150000,
                FeaturesText = "Hall rental (up to 6 hours)\nBasic stage & décor\nChairs, tables & linens\nSound system\nParking attendants",
                DisplayOrder = 1
            },
            new Package
            {
                Name = "Signature",
                Description = "Our most popular package — full décor and catering coordination included.",
                StartingPriceRs = 350000,
                FeaturesText = "Hall rental (up to 8 hours)\nThemed stage & floral décor\nCatering coordination\nSound & lighting\nDedicated event coordinator",
                IsFeatured = true,
                DisplayOrder = 2
            },
            new Package
            {
                Name = "Grand Éclat",
                Description = "A fully managed, premium celebration from arrival to the last dance.",
                StartingPriceRs = 650000,
                FeaturesText = "Full-day hall rental\nPremium stage & décor design\nFull catering service\nProfessional lighting & sound\nPhotography coordination\nDedicated event manager",
                DisplayOrder = 3
            });
    }

    private static async Task SeedTestimonialsAsync(ApplicationDbContext db)
    {
        if (await db.Testimonials.AnyAsync())
        {
            return;
        }

        db.Testimonials.AddRange(
            new Testimonial
            {
                AuthorName = "Ayesha & Bilal",
                EventType = "Wedding",
                Quote = "Gateway Centre made our wedding day effortless. The hall looked stunning and the staff handled everything.",
                Rating = 5,
                DisplayOrder = 1
            },
            new Testimonial
            {
                AuthorName = "Khanpur Chamber of Commerce",
                EventType = "Corporate Event",
                Quote = "Professional venue with great facilities for our annual conference. Will definitely book again.",
                Rating = 5,
                DisplayOrder = 2
            });
    }
}
