using Microsoft.EntityFrameworkCore;
using practo_backend.Models;

namespace practo_backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<OtpRequest> OtpRequests { get; set; }
    public DbSet<Specialty> Specialties { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Clinic> Clinics { get; set; }
    public DbSet<DoctorClinic> DoctorClinics { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<SurgeryCategory> SurgeryCategories { get; set; }
    public DbSet<SurgeryTreatment> SurgeryTreatments { get; set; }
    public DbSet<SurgeryLead> SurgeryLeads { get; set; }
    public DbSet<MedicineCategory> MedicineCategories { get; set; }
    public DbSet<MedicineProduct> MedicineProducts { get; set; }
    public DbSet<DiagnosticTest> DiagnosticTests { get; set; }
    public DbSet<HealthCheckupPackage> HealthCheckupPackages { get; set; }
    public DbSet<HealthConcern> HealthConcerns { get; set; }
    public DbSet<VitalCheckup> VitalCheckups { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<OtpRequest>()
            .HasIndex(o => o.Email);

        modelBuilder.Entity<DoctorClinic>()
            .HasKey(dc => new { dc.DoctorId, dc.ClinicId });

        modelBuilder.Entity<DoctorClinic>()
            .HasOne(dc => dc.Doctor)
            .WithMany(d => d.DoctorClinics)
            .HasForeignKey(dc => dc.DoctorId);

        modelBuilder.Entity<DoctorClinic>()
            .HasOne(dc => dc.Clinic)
            .WithMany(c => c.DoctorClinics)
            .HasForeignKey(dc => dc.ClinicId);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany()
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Patient)
            .WithMany()
            .HasForeignKey(r => r.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Doctor)
            .WithMany(d => d.Reviews)
            .HasForeignKey(r => r.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Specialty>()
            .HasIndex(s => s.Slug)
            .IsUnique();

        modelBuilder.Entity<Appointment>()
            .HasIndex(a => new { a.AppointmentDateTime, a.Status });

        modelBuilder.Entity<SurgeryTreatment>()
            .HasOne(st => st.Category)
            .WithMany(sc => sc.Treatments)
            .HasForeignKey(st => st.SurgeryCategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
