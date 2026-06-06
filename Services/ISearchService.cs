using practo_backend.DTOs;
using practo_backend.Models;

namespace practo_backend.Services;

public interface ISearchService
{
    Task<List<DoctorSearchDto>> SearchDoctorsAsync(string city, string? specialtySlug, string? query, string? gender, decimal? maxFee, string? consultType, string? sortBy);
}
