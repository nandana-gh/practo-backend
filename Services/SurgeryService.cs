using Microsoft.EntityFrameworkCore;
using practo_backend.Data;
using practo_backend.DTOs;
using practo_backend.Models;

namespace practo_backend.Services;

public class SurgeryService : ISurgeryService
{
    private readonly ApplicationDbContext _context;

    public SurgeryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SurgeryCategoryDto>> GetSurgeryCategoriesAsync()
    {
        var categories = await _context.SurgeryCategories
            .Include(c => c.Treatments)
            .ToListAsync();

        return categories.Select(c => new SurgeryCategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Treatments = c.Treatments.Select(t => new SurgeryTreatmentDto
            {
                Id = t.Id,
                Name = t.Name,
                ImageUrl = t.ImageUrl,
                IsPopular = t.IsPopular
            }).ToList()
        }).ToList();
    }

    public async Task<bool> SubmitLeadAsync(SurgeryLeadCreateDto leadDto)
    {
        var lead = new SurgeryLead
        {
            Name = leadDto.Name,
            MobileNumber = leadDto.MobileNumber,
            City = leadDto.City,
            SurgeryName = leadDto.SurgeryName
        };

        await _context.SurgeryLeads.AddAsync(lead);
        var result = await _context.SaveChangesAsync();
        
        return result > 0;
    }
}
