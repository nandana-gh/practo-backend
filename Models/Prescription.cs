using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace practo_backend.Models;

public class Prescription
{
    [Key]
    public int Id { get; set; }

    public int AppointmentId { get; set; }
    
    [ForeignKey("AppointmentId")]
    public Appointment Appointment { get; set; } = null!;

    [Required]
    public string Medications { get; set; } = string.Empty;

    public string? Instructions { get; set; }

    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
}
