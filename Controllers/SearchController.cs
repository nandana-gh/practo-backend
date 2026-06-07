using Microsoft.AspNetCore.Mvc;
using practo_backend.DTOs;
using practo_backend.Services;

namespace practo_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet("doctors")]
    public async Task<ActionResult<List<DoctorSearchDto>>> SearchDoctors(
        [FromQuery] string city = "",
        [FromQuery] string specialtySlug = "",
        [FromQuery] string query = "",
        [FromQuery] string gender = "",
        [FromQuery] decimal? maxFee = null,
        [FromQuery] string consultType = "",
        [FromQuery] string sortBy = "")
    {
        var results = await _searchService.SearchDoctorsAsync(city, specialtySlug, query, gender, maxFee, consultType, sortBy);
        return Ok(results);
    }

    [HttpGet("specialties")]
    public async Task<IActionResult> GetSpecialties([FromServices] practo_backend.Data.ApplicationDbContext context)
    {
        var specialties = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
            System.Linq.Queryable.Select(context.Specialties, s => new { s.Id, s.Name })
        );
        return Ok(specialties);
    }
}
