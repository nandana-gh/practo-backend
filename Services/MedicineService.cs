using Microsoft.EntityFrameworkCore;
using practo_backend.Data;
using practo_backend.DTOs;

namespace practo_backend.Services;

public class MedicineService : IMedicineService
{
    private readonly ApplicationDbContext _context;

    public MedicineService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MedicinesLandingDto> GetLandingDataAsync()
    {
        var allCategories = await _context.MedicineCategories
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new MedicineCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                BackgroundColor = c.BackgroundColor
            })
            .ToListAsync();

        var popularProducts = await _context.MedicineProducts
            .Where(p => p.IsPopular)
            .Select(p => new MedicineProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                OriginalPrice = p.OriginalPrice,
                ImageUrl = p.ImageUrl
            })
            .ToListAsync();

        // Split categories into "Health Conditions" (first 4) and "Categories" (next 4) to match the UI
        return new MedicinesLandingDto
        {
            HealthConditions = allCategories.Take(4).ToList(),
            Categories = allCategories.Skip(4).Take(4).ToList(),
            PopularProducts = popularProducts
        };
    }
}
