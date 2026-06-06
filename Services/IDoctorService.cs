using practo_backend.DTOs;

namespace practo_backend.Services;

public interface IDoctorService
{
    Task<DoctorProfileDto?> GetDoctorProfileAsync(int doctorId);
    Task<List<AppointmentSlotDto>> GetDoctorAvailabilityAsync(int doctorId, int? clinicId, DateTime startDate, int days);
}
