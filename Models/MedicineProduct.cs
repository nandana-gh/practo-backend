using System.ComponentModel.DataAnnotations;

namespace practo_backend.Models;

public class MedicineProduct
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal? OriginalPrice { get; set; }

    [Required]
    [MaxLength(255)]
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsPopular { get; set; } = false;
}
