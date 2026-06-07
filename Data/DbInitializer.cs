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

        // Seed Medicines
        if (!context.MedicineCategories.Any())
        {
            var medicineCategories = new List<MedicineCategory>
            {
                new MedicineCategory { Name = "Skin Care", BackgroundColor = "#9dc7df", DisplayOrder = 1, ImageUrl = "assets/images/cat-skin.png" },
                new MedicineCategory { Name = "Sexual Wellness", BackgroundColor = "#66cdaa", DisplayOrder = 2, ImageUrl = "assets/images/cat-sexual.png" },
                new MedicineCategory { Name = "Weight Management", BackgroundColor = "#a3d36b", DisplayOrder = 3, ImageUrl = "assets/images/cat-weight.png" },
                new MedicineCategory { Name = "Pain Relief", BackgroundColor = "#aa9bc4", DisplayOrder = 4, ImageUrl = "assets/images/cat-pain.png" },
                new MedicineCategory { Name = "Baby Care", BackgroundColor = "#87add3", DisplayOrder = 5, ImageUrl = "assets/images/cat-baby.png" },
                new MedicineCategory { Name = "Fitness & Supplements", BackgroundColor = "#5ac1a9", DisplayOrder = 6, ImageUrl = "assets/images/cat-fitness.png" },
                new MedicineCategory { Name = "Family Nutrition", BackgroundColor = "#84acd9", DisplayOrder = 7, ImageUrl = "assets/images/cat-family.png" },
                new MedicineCategory { Name = "Alternate Medicine", BackgroundColor = "#a39ec8", DisplayOrder = 8, ImageUrl = "assets/images/cat-alternate.png" }
            };
            context.MedicineCategories.AddRange(medicineCategories);
            context.SaveChanges();
        }

        if (!context.MedicineProducts.Any())
        {
            var medicineProducts = new List<MedicineProduct>
            {
                new MedicineProduct { Name = "NAN PRO 1", Description = "400g Powder", Price = 775.00m, IsPopular = true, ImageUrl = "assets/images/prod-nan.png" },
                new MedicineProduct { Name = "Ensure Vanilla", Description = "400g Powder", Price = 450.00m, IsPopular = true, ImageUrl = "assets/images/prod-ensure.png" },
                new MedicineProduct { Name = "Dettol Liquid Handwash", Description = "250ml", Price = 110.00m, IsPopular = true, ImageUrl = "assets/images/prod-dettol.png" },
                new MedicineProduct { Name = "Cerelac Wheat Apple", Description = "300g", Price = 265.00m, IsPopular = true, ImageUrl = "assets/images/prod-cerelac.png" },
                new MedicineProduct { Name = "Himalaya Purifying Neem Face Wash", Description = "150ml", Price = 190.00m, IsPopular = true, ImageUrl = "assets/images/prod-himalaya.png" }
            };
            context.MedicineProducts.AddRange(medicineProducts);
            context.SaveChanges();
        }

        // Seed Surgeries
        if (!await context.SurgeryCategories.AnyAsync())
        {
            var popularCategory = new SurgeryCategory { Name = "Popular" };
            var generalCategory = new SurgeryCategory { Name = "General Surgery" };
            var proctologyCategory = new SurgeryCategory { Name = "Proctology" };
            var ophthalmologyCategory = new SurgeryCategory { Name = "Ophthalmology" };
            var urologyCategory = new SurgeryCategory { Name = "Urology" };
            var cosmeticCategory = new SurgeryCategory { Name = "Cosmetic Surgery" };
            var orthopedicsCategory = new SurgeryCategory { Name = "Orthopedics" };
            var roboticCategory = new SurgeryCategory { Name = "Robotic Surgeries" };
            var oncologyCategory = new SurgeryCategory { Name = "Oncology" };
            var dentalCategory = new SurgeryCategory { Name = "Dental" };

            await context.SurgeryCategories.AddRangeAsync(popularCategory, generalCategory, proctologyCategory, ophthalmologyCategory, urologyCategory, cosmeticCategory, orthopedicsCategory, roboticCategory, oncologyCategory, dentalCategory);
            await context.SaveChangesAsync();

            var treatments = new List<SurgeryTreatment>
            {
                new SurgeryTreatment { Name = "Piles", SurgeryCategoryId = popularCategory.Id, IsPopular = true },
                new SurgeryTreatment { Name = "Varicose Veins", SurgeryCategoryId = popularCategory.Id, IsPopular = true },
                new SurgeryTreatment { Name = "Hernia", SurgeryCategoryId = popularCategory.Id, IsPopular = true },
                new SurgeryTreatment { Name = "Lasik", SurgeryCategoryId = popularCategory.Id, IsPopular = true },
                new SurgeryTreatment { Name = "Gallstone", SurgeryCategoryId = popularCategory.Id, IsPopular = true },
                new SurgeryTreatment { Name = "Anal Fistula", SurgeryCategoryId = popularCategory.Id, IsPopular = true },
                new SurgeryTreatment { Name = "Cataract", SurgeryCategoryId = popularCategory.Id, IsPopular = true },
                new SurgeryTreatment { Name = "Kidney Stone", SurgeryCategoryId = popularCategory.Id, IsPopular = true },
                new SurgeryTreatment { Name = "Circumcision", SurgeryCategoryId = popularCategory.Id, IsPopular = true },
                new SurgeryTreatment { Name = "Anal Fissure", SurgeryCategoryId = popularCategory.Id, IsPopular = true },
                new SurgeryTreatment { Name = "Lipoma Removal", SurgeryCategoryId = popularCategory.Id, IsPopular = true },
                new SurgeryTreatment { Name = "Sebaceous Cyst", SurgeryCategoryId = popularCategory.Id, IsPopular = true },
                new SurgeryTreatment { Name = "Pilonidal Sinus", SurgeryCategoryId = popularCategory.Id, IsPopular = true },
                new SurgeryTreatment { Name = "Lump in Breast", SurgeryCategoryId = popularCategory.Id, IsPopular = true },
                new SurgeryTreatment { Name = "TURP", SurgeryCategoryId = popularCategory.Id, IsPopular = true },
                new SurgeryTreatment { Name = "Hydrocele", SurgeryCategoryId = popularCategory.Id, IsPopular = true },
                new SurgeryTreatment { Name = "Knee Replacement", SurgeryCategoryId = popularCategory.Id, IsPopular = true },
                new SurgeryTreatment { Name = "Hair Transplant", SurgeryCategoryId = popularCategory.Id, IsPopular = true },
                new SurgeryTreatment { Name = "Gynecomastia", SurgeryCategoryId = popularCategory.Id, IsPopular = true }
            };

            await context.SurgeryTreatments.AddRangeAsync(treatments);
            await context.SaveChangesAsync();
        }

        // Seed Lab Tests
        if (!await context.DiagnosticTests.AnyAsync())
        {
            var tests = new List<DiagnosticTest>
            {
                new DiagnosticTest { Name = "Thyroid Profile", KnownAs = "Known as Thyroid Profile Total Blood", Price = 400, OriginalPrice = 500 },
                new DiagnosticTest { Name = "Complete Blood Count", KnownAs = "Known as Complete Blood Count Automated Blood", Price = 300, OriginalPrice = 400 },
                new DiagnosticTest { Name = "Lipid Profile", KnownAs = "Known as Lipid Profile Blood", Price = 434, OriginalPrice = 550 },
                new DiagnosticTest { Name = "Liver Function Test", KnownAs = "Known as Liver Function Tests Blood", Price = 673, OriginalPrice = 800 },
                new DiagnosticTest { Name = "HbA1c", KnownAs = "Known as Glycosylated Haemoglobin Blood", Price = 300, OriginalPrice = 400 },
                new DiagnosticTest { Name = "Vitamin B 12", KnownAs = "Known as Vitamin B12 Conventional Blood", Price = 490, OriginalPrice = 600 }
            };
            await context.DiagnosticTests.AddRangeAsync(tests);
            await context.SaveChangesAsync();
        }

        if (!await context.HealthCheckupPackages.AnyAsync())
        {
            var packages = new List<HealthCheckupPackage>
            {
                new HealthCheckupPackage { Name = "Aarogyam C", ImageUrl = "assets/images/packages/aarogyam-c.jpg", DiscountPercent = 20, IncludedTestsCount = 61, IdealFor = "Adults", Price = 1400, OriginalPrice = 1750 },
                new HealthCheckupPackage { Name = "Comprehensive Women's Health", ImageUrl = "assets/images/packages/womens-health.jpg", DiscountPercent = 20, IncludedTestsCount = 70, IdealFor = "Women", Price = 2200, OriginalPrice = 2750 },
                new HealthCheckupPackage { Name = "Basic Health Screening", ImageUrl = "assets/images/packages/basic-health.jpg", DiscountPercent = 17, IncludedTestsCount = 43, IdealFor = "Adults", Price = 1000, OriginalPrice = 1200 },
                new HealthCheckupPackage { Name = "Comprehensive Gold", ImageUrl = "assets/images/packages/comprehensive-gold.jpg", DiscountPercent = 22, IncludedTestsCount = 50, IdealFor = "Adults", Price = 3500, OriginalPrice = 4500 }
            };
            await context.HealthCheckupPackages.AddRangeAsync(packages);
            await context.SaveChangesAsync();
        }

        if (!await context.HealthConcerns.AnyAsync())
        {
            var concerns = new List<HealthConcern>
            {
                new HealthConcern { Name = "Fever", IconUrl = "assets/images/concerns/fever.png" },
                new HealthConcern { Name = "Diabetes", IconUrl = "assets/images/concerns/diabetes.png" },
                new HealthConcern { Name = "Skin", IconUrl = "assets/images/concerns/skin.png" },
                new HealthConcern { Name = "Kidney", IconUrl = "assets/images/concerns/kidney.png" },
                new HealthConcern { Name = "Digestion", IconUrl = "assets/images/concerns/digestion.png" },
                new HealthConcern { Name = "Cancer", IconUrl = "assets/images/concerns/cancer.png" }
            };
            await context.HealthConcerns.AddRangeAsync(concerns);
            await context.SaveChangesAsync();
        }

        if (!await context.VitalCheckups.AnyAsync())
        {
            var vitals = new List<VitalCheckup>
            {
                new VitalCheckup { Name = "Lipid Profile", IconUrl = "assets/images/vitals/lipid.png", Description = "120 - 150 mins fasting required" },
                new VitalCheckup { Name = "Liver Profile", IconUrl = "assets/images/vitals/liver.png", Description = "Checks overall functionality of liver" },
                new VitalCheckup { Name = "Blood Sugar", IconUrl = "assets/images/vitals/bloodsugar.png", Description = "Fasting Blood Sugar level" }
            };
            await context.VitalCheckups.AddRangeAsync(vitals);
            await context.SaveChangesAsync();
        }

        await context.SaveChangesAsync();
    }
}
