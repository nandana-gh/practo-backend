namespace practo_backend.Models;

public enum ConsultationType
{
    InClinic,
    Video
}

public enum AppointmentStatus
{
    Pending,
    Confirmed,
    Completed,
    Cancelled
}

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public User? Patient { get; set; }
    
    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; }
    
    public int? ClinicId { get; set; }
    public Clinic? Clinic { get; set; }
    
    public DateTime AppointmentDateTime { get; set; }
    public ConsultationType Type { get; set; }
    public AppointmentStatus Status { get; set; }
    public decimal Fee { get; set; }
    public string ReasonForVisit { get; set; } = string.Empty;
    
    public bool IsForFamilyMember { get; set; }
    public string PatientName { get; set; } = string.Empty; // Used if IsForFamilyMember is true
    public string PatientAge { get; set; } = string.Empty;
    public string PatientGender { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string RazorpayOrderId { get; set; } = string.Empty;
    public string RazorpayPaymentId { get; set; } = string.Empty;
}
