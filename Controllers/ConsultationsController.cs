using Microsoft.AspNetCore.Mvc;
using practo_backend.DTOs;

namespace practo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultationsController : ControllerBase
    {
        [HttpPost("request")]
        public IActionResult SubmitConsultRequest([FromBody] ConsultRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Symptom) || request.Symptom.Length < 4)
            {
                return BadRequest(new { message = "Symptom must be at least 4 characters." });
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { message = "Email address is required." });
            }

            // In a real app, this would save to the database and send an OTP.
            return Ok(new { message = "Consultation request submitted successfully! A verified doctor will be assigned shortly." });
        }
    }
}
