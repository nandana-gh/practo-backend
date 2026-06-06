namespace practo_backend.Models;

public class Doctor
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    
    public int SpecialtyId { get; set; }
    public Specialty? Specialty { get; set; }
    
    public string Qualifications { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string LanguagesSpoken { get; set; } = string.Empty;
    public decimal VideoConsultationFee { get; set; }
    public bool IsVideoConsultationAvailable { get; set; }
    public string ProfileImageUrl { get; set; } = string.Empty;
    public string About { get; set; } = string.Empty;
    public double RecommendationPercentage { get; set; }
    
    public ICollection<DoctorClinic> DoctorClinics { get; set; } = new List<DoctorClinic>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
