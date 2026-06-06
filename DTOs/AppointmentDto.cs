using practo_backend.Models;

namespace practo_backend.DTOs;

public class AppointmentSlotDto
{
    public DateTime Date { get; set; }
    public List<string> AvailableTimes { get; set; } = new();
}

public class BookAppointmentDto
{
    public int DoctorId { get; set; }
    public int? ClinicId { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public ConsultationType Type { get; set; }
    public string ReasonForVisit { get; set; } = string.Empty;
    public bool IsForFamilyMember { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string PatientAge { get; set; } = string.Empty;
    public string PatientGender { get; set; } = string.Empty;
}
