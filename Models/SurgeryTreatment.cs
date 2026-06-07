namespace practo_backend.Models;

public class SurgeryTreatment
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int SurgeryCategoryId { get; set; }
    public SurgeryCategory? Category { get; set; }
    public bool IsPopular { get; set; }
}
