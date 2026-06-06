namespace practo_backend.Models;

public class Review
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; }
    
    public int PatientId { get; set; }
    public User? Patient { get; set; }
    
    public int Rating { get; set; } // 1 to 5
    public string Feedback { get; set; } = string.Empty;
    public bool IsRecommended { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
