using practo_backend.Models;

namespace practo_backend.DTOs;

public class AppointmentDetailsDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string DoctorSpecialty { get; set; } = string.Empty;
    public int? ClinicId { get; set; }
    public string ClinicName { get; set; } = string.Empty;
    public DateTime AppointmentDateTime { get; set; }
    public ConsultationType Type { get; set; }
    public AppointmentStatus Status { get; set; }
    public string ReasonForVisit { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
