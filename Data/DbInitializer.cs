using Microsoft.EntityFrameworkCore;
using practo_backend.Models;
using practo_backend.Services;

namespace practo_backend.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Apply migrations automatically
        await context.Database.MigrateAsync();

        // Seed Specialties
        if (!await context.Specialties.AnyAsync())
        {
            var specialties = new List<Specialty>
            {
                new Specialty { Name = "Dentist", Slug = "dentist", Description = "Teeth and gum care, root canals, fillings, and braces.", ImageUrl = "assets/images/specialties/dentist.jpg" },
                new Specialty { Name = "Gynecologist", Slug = "gynecologist", Description = "Female reproductive health, pregnancy, and childbirth.", ImageUrl = "assets/images/specialties/gynecologist.jpg" },
                new Specialty { Name = "Dietitian", Slug = "dietitian", Description = "Nutrition plans, weight management, and dietary advice.", ImageUrl = "assets/images/specialties/dietitian.jpg" },
                new Specialty { Name = "Physiotherapist", Slug = "physiotherapist", Description = "Physical rehabilitation, joint pain relief, and exercise plans.", ImageUrl = "assets/images/specialties/physiotherapist.jpg" },
                new Specialty { Name = "General Physician", Slug = "general-physician", Description = "General health consultations, fever, infections, and cold.", ImageUrl = "assets/images/specialties/general-physician.jpg" },
                new Specialty { Name = "Dermatologist", Slug = "dermatologist", Description = "Skin, hair, nails, acne, eczema, and hair loss treatment.", ImageUrl = "assets/images/specialties/dermatologist.jpg" },
                new Specialty { Name = "Psychiatrist", Slug = "psychiatrist", Description = "Mental health, anxiety, depression, and therapy guidance.", ImageUrl = "assets/images/specialties/psychiatrist.jpg" }
            };

            await context.Specialties.AddRangeAsync(specialties);
        }

        // Seed Admin User
        if (!await context.Users.AnyAsync(u => u.Email == "admin@practo.com"))
        {
            var admin = new User
            {
                FirstName = "System",
                LastName = "Admin",
                Email = "admin@practo.com",
                PasswordHash = PasswordHasher.HashPassword("AdminPassword123!"),
                PhoneNumber = "1234567890",
                Role = UserRole.Admin,
                IsVerified = true,
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(admin);
        }

        await context.SaveChangesAsync();
    }
}
