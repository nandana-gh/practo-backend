using Microsoft.EntityFrameworkCore;
using practo_backend.Data;
using practo_backend.DTOs;

namespace practo_backend.Services;

public class SearchService : ISearchService
{
    private readonly ApplicationDbContext _context;

    public SearchService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DoctorSearchDto>> SearchDoctorsAsync(string city, string? specialtySlug, string? query, string? gender, decimal? maxFee, string? consultType, string? sortBy)
    {
        var docsQuery = _context.Doctors
            .Include(d => d.User)
            .Include(d => d.Specialty)
            .Include(d => d.DoctorClinics)
                .ThenInclude(dc => dc.Clinic)
            .Where(d => d.IsApproved)
            .AsQueryable();

        // City filter
        if (!string.IsNullOrEmpty(city))
        {
            docsQuery = docsQuery.Where(d => 
                d.DoctorClinics.Any(dc => dc.Clinic.City.ToLower() == city.ToLower()) || 
                d.DoctorClinics.Count == 0); // Include if they don't have a clinic yet
        }

        // Specialty filter
        if (!string.IsNullOrEmpty(specialtySlug))
        {
            docsQuery = docsQuery.Where(d => d.Specialty.Slug.ToLower() == specialtySlug.ToLower());
        }

        // Query filter (Name, Speciality, or Symptoms)
        if (!string.IsNullOrEmpty(query))
        {
            var lowerQuery = query.ToLower();
            docsQuery = docsQuery.Where(d => 
                (d.User.FirstName + " " + d.User.LastName).ToLower().Contains(lowerQuery) ||
                d.Specialty.Name.ToLower().Contains(lowerQuery) ||
                d.About.ToLower().Contains(lowerQuery)
            );
        }

        // Gender filter
        if (!string.IsNullOrEmpty(gender))
        {
            // Assuming gender info is added or filtered via a heuristic or added to user. We can skip or implement if we add gender to User.
            // For now, let's assume it's not strictly filtered if Gender isn't in User model yet.
        }

        // Max Fee filter
        if (maxFee.HasValue)
        {
            docsQuery = docsQuery.Where(d => 
                d.DoctorClinics.Any(dc => dc.ConsultationFee <= maxFee.Value) || 
                (d.IsVideoConsultationAvailable && d.VideoConsultationFee <= maxFee.Value)
            );
        }

        // Consult Type
        if (!string.IsNullOrEmpty(consultType) && consultType.ToLower() == "video")
        {
            docsQuery = docsQuery.Where(d => d.IsVideoConsultationAvailable);
        }

        var results = await docsQuery.Select(d => new DoctorSearchDto
        {
            Id = d.Id,
            Name = "Dr. " + d.User.FirstName + " " + d.User.LastName,
            Specialty = d.Specialty.Name,
            ExperienceYears = d.ExperienceYears,
            ClinicName = d.DoctorClinics.FirstOrDefault() != null ? d.DoctorClinics.FirstOrDefault().Clinic.Name : "",
            Locality = d.DoctorClinics.FirstOrDefault() != null ? d.DoctorClinics.FirstOrDefault().Clinic.Locality : "",
            City = d.DoctorClinics.FirstOrDefault() != null ? d.DoctorClinics.FirstOrDefault().Clinic.City : "",
            ConsultationFee = d.DoctorClinics.FirstOrDefault() != null ? d.DoctorClinics.FirstOrDefault().ConsultationFee : d.VideoConsultationFee,
            IsVideoConsultationAvailable = d.IsVideoConsultationAvailable,
            RecommendationPercentage = d.RecommendationPercentage,
            TotalReviews = d.Reviews.Count,
            ProfileImageUrl = d.ProfileImageUrl
        }).ToListAsync();

        // Sort By
        if (!string.IsNullOrEmpty(sortBy))
        {
            results = sortBy.ToLower() switch
            {
                "experience" => results.OrderByDescending(r => r.ExperienceYears).ToList(),
                "fee" => results.OrderBy(r => r.ConsultationFee).ToList(),
                "rating" => results.OrderByDescending(r => r.RecommendationPercentage).ToList(),
                _ => results // Relevance (default)
            };
        }

        return results;
    }
}
