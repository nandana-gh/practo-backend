using System.ComponentModel.DataAnnotations;

namespace practo_backend.DTOs;

public class SurgeryCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<SurgeryTreatmentDto> Treatments { get; set; } = new();
}

public class SurgeryTreatmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPopular { get; set; }
}

public class SurgeryLeadCreateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string MobileNumber { get; set; } = string.Empty;
    [Required]
    public string City { get; set; } = string.Empty;
    [Required]
    public string SurgeryName { get; set; } = string.Empty;
}
