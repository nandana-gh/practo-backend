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

    public async Task<int?> GetDoctorIdByUserIdAsync(int userId)
    {
        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
        return doctor?.Id;
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
            }).ToList(),
            VideoConsultationFee = doctor.VideoConsultationFee
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

    public async Task<List<DoctorPatientDto>> GetDoctorPatientsAsync(int doctorId)
    {
        var appointments = await _context.Appointments
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId)
            .ToListAsync();

        var patients = appointments
            .GroupBy(a => a.PatientId)
            .Select(g => new DoctorPatientDto
            {
                Id = g.Key,
                Name = g.First().Patient.FirstName + " " + g.First().Patient.LastName,
                Email = g.First().Patient.Email,
                PhoneNumber = g.First().Patient.PhoneNumber,
                LastVisitDate = g.Max(a => a.AppointmentDateTime),
                TotalVisits = g.Count()
            })
            .OrderByDescending(p => p.LastVisitDate)
            .ToList();

        return patients;
    }

    public async Task<DoctorReportDto> GetDoctorReportsAsync(int doctorId)
    {
        var appointments = await _context.Appointments
            .Where(a => a.DoctorId == doctorId)
            .ToListAsync();

        return new DoctorReportDto
        {
            TotalAppointments = appointments.Count,
            TotalPatients = appointments.Select(a => a.PatientId).Distinct().Count(),
            TotalEarnings = appointments.Where(a => a.Status != AppointmentStatus.Cancelled).Sum(a => a.Fee),
            UpcomingAppointments = appointments.Count(a => a.AppointmentDateTime > DateTime.UtcNow && a.Status != AppointmentStatus.Cancelled)
        };
    }

    public async Task<bool> UpdateDoctorProfileAsync(int doctorId, DoctorProfileUpdateDto dto)
    {
        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == doctorId);
        if (doctor == null) return false;

        doctor.Qualifications = dto.Qualifications;
        doctor.ExperienceYears = dto.ExperienceYears;
        doctor.RegistrationNumber = dto.RegistrationNumber;
        doctor.LanguagesSpoken = dto.LanguagesSpoken;
        doctor.About = dto.About;
        doctor.VideoConsultationFee = dto.VideoConsultationFee;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddClinicToDoctorAsync(int doctorId, DoctorClinicCreateDto dto)
    {
        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == doctorId);
        if (doctor == null) return false;

        var clinic = new Clinic
        {
            Name = dto.Name,
            Address = dto.Address,
            City = dto.City,
            Locality = dto.Locality,
            Timings = dto.Timings,
            ImageUrl = "assets/images/clinics/default-clinic.png", // Hardcode default
            Latitude = 0, // Should be geocoded realistically
            Longitude = 0
        };

        await _context.Clinics.AddAsync(clinic);
        await _context.SaveChangesAsync();

        var doctorClinic = new DoctorClinic
        {
            DoctorId = doctorId,
            ClinicId = clinic.Id,
            ConsultationFee = dto.ConsultationFee
        };

        await _context.DoctorClinics.AddAsync(doctorClinic);
        await _context.SaveChangesAsync();

        return true;
    }
}
