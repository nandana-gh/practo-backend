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
}
