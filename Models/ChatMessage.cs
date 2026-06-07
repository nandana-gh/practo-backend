using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace practo_backend.Models;

public class ChatMessage
{
    [Key]
    public int Id { get; set; }

    public int AppointmentId { get; set; }
    
    [ForeignKey("AppointmentId")]
    public Appointment Appointment { get; set; } = null!;

    public int SenderId { get; set; }
    
    [ForeignKey("SenderId")]
    public User Sender { get; set; } = null!;

    [Required]
    public string Message { get; set; } = string.Empty;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
