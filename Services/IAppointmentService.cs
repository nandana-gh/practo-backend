using practo_backend.DTOs;
using practo_backend.Models;

namespace practo_backend.Services;

public interface IAppointmentService
{
    Task<Appointment?> BookAppointmentAsync(int patientId, BookAppointmentDto dto);
}
