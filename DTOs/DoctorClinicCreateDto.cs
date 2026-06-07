namespace practo_backend.DTOs;

public class DoctorClinicCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Locality { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
    public decimal ConsultationFee { get; set; }
}
