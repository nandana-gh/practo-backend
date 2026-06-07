namespace practo_backend.DTOs;

public class MedicineCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string BackgroundColor { get; set; } = string.Empty;
}
