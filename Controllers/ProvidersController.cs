using Microsoft.AspNetCore.Mvc;
using PractoBackend.DTOs;
using System.Threading.Tasks;

namespace PractoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProvidersController : ControllerBase
    {
        [HttpPost("join")]
        public async Task<IActionResult> SubmitLead([FromBody] ProviderLeadDto leadDto)
        {
            if (leadDto == null)
            {
                return BadRequest("Invalid lead data.");
            }

            // In a real application, we would save this to a database, send an email to the sales team, etc.
            // For now, we will simulate a successful processing.
            await Task.Delay(1000); // Simulate network/processing delay

            return Ok(new { message = "Thank you for your interest! Our team will contact you shortly." });
        }

        [HttpPost("profile-lead")]
        public async Task<IActionResult> SubmitProfileLead([FromBody] ProfileLeadDto leadDto)
        {
            if (leadDto == null)
            {
                return BadRequest("Invalid lead data.");
            }

            // Simulate processing
            await Task.Delay(1000); 

            return Ok(new { message = "Profile lead submitted successfully." });
        }
    }
}
