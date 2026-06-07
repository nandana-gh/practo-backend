using Microsoft.AspNetCore.Mvc;
using practo_backend.DTOs;
using practo_backend.Services;

namespace practo_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SurgeryController : ControllerBase
{
    private readonly ISurgeryService _surgeryService;

    public SurgeryController(ISurgeryService surgeryService)
    {
        _surgeryService = surgeryService;
    }

    [HttpGet("categories")]
    public async Task<ActionResult<List<SurgeryCategoryDto>>> GetCategories()
    {
        var categories = await _surgeryService.GetSurgeryCategoriesAsync();
        return Ok(categories);
    }

    [HttpPost("lead")]
    public async Task<ActionResult> SubmitLead([FromBody] SurgeryLeadCreateDto leadDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _surgeryService.SubmitLeadAsync(leadDto);
        if (result)
            return Ok(new { message = "Lead submitted successfully." });

        return BadRequest("Failed to submit lead.");
    }
}
