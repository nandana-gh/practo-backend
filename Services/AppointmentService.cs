using Microsoft.EntityFrameworkCore;
using practo_backend.Data;
using practo_backend.DTOs;
using practo_backend.Models;

namespace practo_backend.Services;

public class AppointmentService : IAppointmentService
{
    private readonly ApplicationDbContext _context;

    public AppointmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Appointment?> BookAppointmentAsync(int patientId, BookAppointmentDto dto)
    {
        // 1. Transaction to prevent double booking
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Check if slot is already booked for this doctor
            var existingBooking = await _context.Appointments
                .Where(a => a.DoctorId == dto.DoctorId 
                            && a.AppointmentDateTime == dto.AppointmentDateTime 
                            && a.Status != AppointmentStatus.Cancelled)
                .FirstOrDefaultAsync();

            if (existingBooking != null)
            {
                // Slot is taken
                return null;
            }

            // Calculate Fee
            decimal fee = 0;
            if (dto.Type == ConsultationType.InClinic && dto.ClinicId.HasValue)
            {
                var docClinic = await _context.DoctorClinics.FirstOrDefaultAsync(dc => dc.DoctorId == dto.DoctorId && dc.ClinicId == dto.ClinicId);
                if (docClinic != null) fee = docClinic.ConsultationFee;
            }
            else if (dto.Type == ConsultationType.Video)
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == dto.DoctorId);
                if (doctor != null) fee = doctor.VideoConsultationFee;
            }

            // Create Appointment
            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = dto.DoctorId,
                ClinicId = dto.Type == ConsultationType.InClinic ? dto.ClinicId : null,
                AppointmentDateTime = dto.AppointmentDateTime,
                Type = dto.Type,
                Status = AppointmentStatus.Pending, // Will be Confirmed after Payment in real scenario
                Fee = fee,
                ReasonForVisit = dto.ReasonForVisit,
                IsForFamilyMember = dto.IsForFamilyMember,
                PatientName = dto.PatientName,
                PatientAge = dto.PatientAge,
                PatientGender = dto.PatientGender,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
            
            await transaction.CommitAsync();

            return appointment;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<AppointmentDetailsDto>> GetPatientAppointmentsAsync(int patientId)
    {
        return await _context.Appointments
            .Include(a => a.Doctor)
                .ThenInclude(d => d.Specialty)
            .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
            .Include(a => a.Clinic)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDateTime)
            .Select(a => new AppointmentDetailsDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = a.PatientName,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.User != null ? a.Doctor.User.FirstName + " " + a.Doctor.User.LastName : "",
                DoctorSpecialty = a.Doctor.Specialty != null ? a.Doctor.Specialty.Name : "",
                ClinicId = a.ClinicId,
                ClinicName = a.Clinic != null ? a.Clinic.Name : "",
                AppointmentDateTime = a.AppointmentDateTime,
                Type = a.Type,
                Status = a.Status,
                ReasonForVisit = a.ReasonForVisit,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<AppointmentDetailsDto>> GetDoctorAppointmentsAsync(int doctorId)
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
            .Include(a => a.Clinic)
            .Where(a => a.DoctorId == doctorId)
            .OrderByDescending(a => a.AppointmentDateTime)
            .Select(a => new AppointmentDetailsDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = a.PatientName,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.User != null ? a.Doctor.User.FirstName + " " + a.Doctor.User.LastName : "",
                DoctorSpecialty = "",
                ClinicId = a.ClinicId,
                ClinicName = a.Clinic != null ? a.Clinic.Name : "",
                AppointmentDateTime = a.AppointmentDateTime,
                Type = a.Type,
                Status = a.Status,
                ReasonForVisit = a.ReasonForVisit,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();
    }
}
