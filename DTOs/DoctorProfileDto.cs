using practo_backend.Models;

namespace practo_backend.DTOs;

public class DoctorProfileDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string Qualifications { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string LanguagesSpoken { get; set; } = string.Empty;
    public string About { get; set; } = string.Empty;
    public double RecommendationPercentage { get; set; }
    public string ProfileImageUrl { get; set; } = string.Empty;
    
    public List<ClinicDto> Clinics { get; set; } = new();
    public decimal VideoConsultationFee { get; set; }
}

public class ClinicDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Locality { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
    public decimal ConsultationFee { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
