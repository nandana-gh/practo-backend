using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using practo_backend.DTOs;
using practo_backend.Services;

namespace practo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultationsController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IEmailService _emailService;

        public ConsultationsController(IMemoryCache cache, IEmailService emailService)
        {
            _cache = cache;
            _emailService = emailService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> SubmitConsultRequest([FromBody] ConsultRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Symptom) || request.Symptom.Length < 4)
            {
                return BadRequest(new { message = "Symptom must be at least 4 characters." });
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { message = "Email address is required." });
            }

            // Generate 6-digit OTP
            var otp = new Random().Next(100000, 999999).ToString();

            // Store in cache for 10 minutes
            _cache.Set($"OTP_{request.Email}", otp, TimeSpan.FromMinutes(10));

            try
            {
                // Send OTP via Email
                await _emailService.SendOtpAsync(request.Email, otp);
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                // Remove from cache if sending failed
                _cache.Remove($"OTP_{request.Email}");
                return StatusCode(500, new { message = "Failed to send OTP email. Please check the SMTP credentials in appsettings.Development.json: " + ex.Message });
            }
            catch (Exception ex)
            {
                _cache.Remove($"OTP_{request.Email}");
                return StatusCode(500, new { message = "An error occurred while sending the email." });
            }

            return Ok(new { message = "OTP sent successfully. Please check your email." });
        }

        [HttpPost("verify-otp")]
        public IActionResult VerifyOtp([FromBody] VerifyOtpDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Otp))
            {
                return BadRequest(new { message = "Email and OTP are required." });
            }

            if (_cache.TryGetValue($"OTP_{request.Email}", out string? storedOtp))
            {
                if (storedOtp == request.Otp)
                {
                    // Clear the OTP so it can't be reused
                    _cache.Remove($"OTP_{request.Email}");
                    return Ok(new { message = "Consultation request submitted successfully! A verified doctor will be assigned shortly." });
                }
                
                return BadRequest(new { message = "Invalid OTP. Please try again." });
            }

            return BadRequest(new { message = "OTP expired or not found. Please request a new one." });
        }
    }
}
