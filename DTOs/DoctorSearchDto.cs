namespace practo_backend.DTOs;

public class DoctorSearchDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string ClinicName { get; set; } = string.Empty;
    public string Locality { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public decimal ConsultationFee { get; set; }
    public bool IsVideoConsultationAvailable { get; set; }
    public double RecommendationPercentage { get; set; }
    public int TotalReviews { get; set; }
    public string ProfileImageUrl { get; set; } = string.Empty;
}
