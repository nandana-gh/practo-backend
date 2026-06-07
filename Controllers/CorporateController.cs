using Microsoft.AspNetCore.Mvc;
using practo_backend.DTOs;

namespace practo_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CorporateController : ControllerBase
{
    private readonly ILogger<CorporateController> _logger;

    public CorporateController(ILogger<CorporateController> logger)
    {
        _logger = logger;
    }

    [HttpPost("demo")]
    public IActionResult SubmitDemoRequest([FromBody] CorporateDemoDto request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest(new { message = "Invalid demo request data." });
            }

            // In a real application, you would save this to a database or send an email.
            // For now, we will just log the request and return success.
            _logger.LogInformation($"New Corporate Demo Request from {request.Name} ({request.OrganizationName}). " +
                                   $"Email: {request.OfficialEmailId}, Phone: {request.MobileNumber}, " +
                                   $"Size: {request.OrganizationSize}, City: {request.City}");

            return Ok(new { message = "Demo request submitted successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting demo request.");
            return StatusCode(500, new { message = "An error occurred while submitting the demo request." });
        }
    }
}
