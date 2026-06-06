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

        // Seed Clinics
        if (!await context.Clinics.AnyAsync())
        {
            var clinics = new List<Clinic>
            {
                new Clinic { Name = "Practo Care Surgeries", Address = "123 Health Ave", City = "Bangalore", Locality = "Koramangala", Latitude = 12.9352, Longitude = 77.6245, Timings = "Mon-Sat, 09:00 AM - 08:00 PM", ImageUrl = "assets/images/clinics/clinic1.jpg" },
                new Clinic { Name = "Apollo Clinic", Address = "45 Wellness Blvd", City = "Mumbai", Locality = "Andheri", Latitude = 19.1136, Longitude = 72.8697, Timings = "Mon-Sun, 24 Hours", ImageUrl = "assets/images/clinics/clinic2.jpg" }
            };
            await context.Clinics.AddRangeAsync(clinics);
            await context.SaveChangesAsync();
        }

        // Seed Doctors
        if (!await context.Doctors.AnyAsync())
        {
            var dentistSpecialty = await context.Specialties.FirstOrDefaultAsync(s => s.Slug == "dentist");
            var generalPhysician = await context.Specialties.FirstOrDefaultAsync(s => s.Slug == "general-physician");

            if (dentistSpecialty != null && generalPhysician != null)
            {
                var docUser1 = new User { FirstName = "Sarah", LastName = "Connor", Email = "dr.sarah@practo.com", PasswordHash = PasswordHasher.HashPassword("DocPassword123!"), PhoneNumber = "9876543210", Role = UserRole.Doctor, IsVerified = true };
                var docUser2 = new User { FirstName = "John", LastName = "Watson", Email = "dr.john@practo.com", PasswordHash = PasswordHasher.HashPassword("DocPassword123!"), PhoneNumber = "9876543211", Role = UserRole.Doctor, IsVerified = true };

                await context.Users.AddRangeAsync(docUser1, docUser2);
                await context.SaveChangesAsync();

                var doctor1 = new Doctor { UserId = docUser1.Id, SpecialtyId = dentistSpecialty.Id, Qualifications = "BDS, MDS - Oral & Maxillofacial Surgery", ExperienceYears = 12, RegistrationNumber = "DENT12345", LanguagesSpoken = "English, Hindi", VideoConsultationFee = 500, IsVideoConsultationAvailable = true, RecommendationPercentage = 98, ProfileImageUrl = "assets/images/doctors/doc1.jpg", About = "Dr. Sarah is an expert in Root Canals and Implants." };
                var doctor2 = new Doctor { UserId = docUser2.Id, SpecialtyId = generalPhysician.Id, Qualifications = "MBBS, MD - General Medicine", ExperienceYears = 8, RegistrationNumber = "GEN67890", LanguagesSpoken = "English, Marathi", VideoConsultationFee = 300, IsVideoConsultationAvailable = true, RecommendationPercentage = 95, ProfileImageUrl = "assets/images/doctors/doc2.jpg", About = "Dr. John has extensive experience in treating viral fevers and lifestyle disorders." };

                await context.Doctors.AddRangeAsync(doctor1, doctor2);
                await context.SaveChangesAsync();

                var clinic1 = await context.Clinics.FirstAsync(c => c.City == "Bangalore");
                var clinic2 = await context.Clinics.FirstAsync(c => c.City == "Mumbai");

                var docClinics = new List<DoctorClinic>
                {
                    new DoctorClinic { DoctorId = doctor1.Id, ClinicId = clinic1.Id, ConsultationFee = 600 },
                    new DoctorClinic { DoctorId = doctor2.Id, ClinicId = clinic2.Id, ConsultationFee = 400 }
                };

                await context.DoctorClinics.AddRangeAsync(docClinics);
                await context.SaveChangesAsync();
            }
        }

        await context.SaveChangesAsync();
    }
}
