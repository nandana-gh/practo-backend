namespace practo_backend.DTOs;

public class DoctorPatientDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime LastVisitDate { get; set; }
    public int TotalVisits { get; set; }
}

public class DoctorReportDto
{
    public int TotalPatients { get; set; }
    public int TotalAppointments { get; set; }
    public decimal TotalEarnings { get; set; }
    public int UpcomingAppointments { get; set; }
}

public class DoctorProfileUpdateDto
{
    public string Qualifications { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string LanguagesSpoken { get; set; } = string.Empty;
    public string About { get; set; } = string.Empty;
    public decimal VideoConsultationFee { get; set; }
}
