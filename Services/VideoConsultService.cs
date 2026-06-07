using Microsoft.EntityFrameworkCore;
using practo_backend.Data;
using practo_backend.DTOs;

namespace practo_backend.Services;

public class VideoConsultService : IVideoConsultService
{
    private readonly ApplicationDbContext _context;

    public VideoConsultService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VideoConsultLandingDto> GetLandingPageDataAsync()
    {
        // 1. Fetch Top 6 Specialties (mocking starting price since it's not in db)
        var specialties = await _context.Specialties
            .Take(6)
            .Select(s => new SpecialtyCardDto
            {
                Id = s.Id,
                Name = s.Name,
                Slug = s.Slug,
                StartingPrice = 399m // Mocked base price for video consults
            })
            .ToListAsync();

        // 2. Fetch Top 4 Doctors where IsVideoConsultationAvailable == true
        var doctors = await _context.Doctors
            .Include(d => d.User)
            .Include(d => d.Specialty)
            .Where(d => d.IsVideoConsultationAvailable)
            .OrderByDescending(d => d.RecommendationPercentage)
            .Take(4)
            .Select(d => new DoctorCardDto
            {
                Id = d.Id,
                Name = d.User!.FirstName + " " + d.User!.LastName,
                Specialty = d.Specialty!.Name,
                ExperienceYears = d.ExperienceYears,
                ProfileImageUrl = d.ProfileImageUrl
            })
            .ToListAsync();

        // 3. Construct Stats (static for now, could be dynamic in a real app)
        var stats = new VideoConsultStatsDto();

        return new VideoConsultLandingDto
        {
            Specialties = specialties,
            Doctors = doctors,
            Stats = stats
        };
    }
}
