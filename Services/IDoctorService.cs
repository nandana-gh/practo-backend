using practo_backend.DTOs;

namespace practo_backend.Services;

public interface IDoctorService
{
    Task<DoctorProfileDto?> GetDoctorProfileAsync(int doctorId);
    Task<int?> GetDoctorIdByUserIdAsync(int userId);
    Task<List<AppointmentSlotDto>> GetDoctorAvailabilityAsync(int doctorId, int? clinicId, DateTime startDate, int days);
    Task<List<DoctorPatientDto>> GetDoctorPatientsAsync(int doctorId);
    Task<DoctorReportDto> GetDoctorReportsAsync(int doctorId);
    Task<bool> UpdateDoctorProfileAsync(int doctorId, DoctorProfileUpdateDto dto);
    Task<bool> AddClinicToDoctorAsync(int doctorId, DoctorClinicCreateDto dto);
}
