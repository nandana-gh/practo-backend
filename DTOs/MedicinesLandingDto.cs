namespace practo_backend.DTOs;

public class MedicinesLandingDto
{
    public List<MedicineCategoryDto> HealthConditions { get; set; } = new();
    public List<MedicineCategoryDto> Categories { get; set; } = new();
    public List<MedicineProductDto> PopularProducts { get; set; } = new();
}
