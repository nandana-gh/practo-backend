using Microsoft.EntityFrameworkCore;
using practo_backend.Data;
using practo_backend.DTOs;
using practo_backend.Models;

namespace practo_backend.Services;

public class DoctorService : IDoctorService
{
    private readonly ApplicationDbContext _context;

    public DoctorService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DoctorProfileDto?> GetDoctorProfileAsync(int doctorId)
    {
        var doctor = await _context.Doctors
            .Include(d => d.User)
            .Include(d => d.Specialty)
            .Include(d => d.DoctorClinics)
                .ThenInclude(dc => dc.Clinic)
            .Where(d => d.IsApproved)
            .FirstOrDefaultAsync(d => d.Id == doctorId);

        if (doctor == null) return null;

        return new DoctorProfileDto
        {
            Id = doctor.Id,
            Name = "Dr. " + doctor.User.FirstName + " " + doctor.User.LastName,
            Specialty = doctor.Specialty.Name,
            Qualifications = doctor.Qualifications,
            ExperienceYears = doctor.ExperienceYears,
            RegistrationNumber = doctor.RegistrationNumber,
            LanguagesSpoken = doctor.LanguagesSpoken,
            About = doctor.About,
            RecommendationPercentage = doctor.RecommendationPercentage,
            ProfileImageUrl = doctor.ProfileImageUrl,
            Clinics = doctor.DoctorClinics.Select(dc => new ClinicDto
            {
                Id = dc.Clinic.Id,
                Name = dc.Clinic.Name,
                Address = dc.Clinic.Address,
                City = dc.Clinic.City,
                Locality = dc.Clinic.Locality,
                Timings = dc.Clinic.Timings,
                ConsultationFee = dc.ConsultationFee,
                ImageUrl = dc.Clinic.ImageUrl,
                Latitude = dc.Clinic.Latitude,
                Longitude = dc.Clinic.Longitude
            }).ToList()
        };
    }

    public async Task<List<AppointmentSlotDto>> GetDoctorAvailabilityAsync(int doctorId, int? clinicId, DateTime startDate, int days)
    {
        var slots = new List<AppointmentSlotDto>();

        // Get booked appointments for the doctor in the given date range to subtract from available slots
        var endDate = startDate.AddDays(days);
        var bookedAppointments = await _context.Appointments
            .Where(a => a.DoctorId == doctorId && a.AppointmentDateTime >= startDate && a.AppointmentDateTime < endDate && a.Status != AppointmentStatus.Cancelled)
            .Select(a => a.AppointmentDateTime)
            .ToListAsync();

        for (int i = 0; i < days; i++)
        {
            var currentDate = startDate.AddDays(i).Date;
            var availableTimes = new List<string>();

            // Mocked slot generation (9 AM to 5 PM, every 30 mins)
            var timeCursor = currentDate.AddHours(9); // 9 AM
            var endTime = currentDate.AddHours(17); // 5 PM

            while (timeCursor < endTime)
            {
                // If not booked and not in the past, add to available
                if (!bookedAppointments.Contains(timeCursor) && timeCursor > DateTime.Now)
                {
                    availableTimes.Add(timeCursor.ToString("HH:mm"));
                }
                timeCursor = timeCursor.AddMinutes(30);
            }

            slots.Add(new AppointmentSlotDto
            {
                Date = currentDate,
                AvailableTimes = availableTimes
            });
        }

        return slots;
    }
}
