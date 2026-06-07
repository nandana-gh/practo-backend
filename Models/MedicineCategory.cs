using System.ComponentModel.DataAnnotations;

namespace practo_backend.Models;

public class MedicineCategory
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [MaxLength(255)]
    public string ImageUrl { get; set; } = string.Empty;

    [MaxLength(50)]
    public string BackgroundColor { get; set; } = "#ffffff";
    
    public int DisplayOrder { get; set; }
}
