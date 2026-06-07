using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PractoBackend.DTOs;
using practo_backend.Services;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;

namespace PractoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProvidersController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IEmailService _emailService;

        public ProvidersController(IMemoryCache cache, IEmailService emailService)
        {
            _cache = cache;
            _emailService = emailService;
        }

        [HttpPost("join")]
        public async Task<IActionResult> SubmitLead([FromBody] ProviderLeadDto leadDto)
        {
            if (leadDto == null)
            {
                return BadRequest("Invalid lead data.");
            }

            await Task.Delay(1000); 

            return Ok(new { message = "Thank you for your interest! Our team will contact you shortly." });
        }

        [HttpPost("profile-lead")]
        public async Task<IActionResult> SubmitProfileLead([FromBody] ProfileLeadDto leadDto)
        {
            if (string.IsNullOrWhiteSpace(leadDto.Name) || string.IsNullOrWhiteSpace(leadDto.Phone))
            {
                return BadRequest(new { message = "Name and Email are required." });
            }
            
            // Note: leadDto.Phone is now being used to pass the Email address from the frontend

            var otp = new Random().Next(100000, 999999).ToString();
            _cache.Set($"PROVIDER_OTP_{leadDto.Phone}", otp, TimeSpan.FromMinutes(10));

            try
            {
                await _emailService.SendOtpAsync(leadDto.Phone, otp);
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                _cache.Remove($"PROVIDER_OTP_{leadDto.Phone}");
                return StatusCode(500, new { message = "Failed to send OTP email: " + ex.Message });
            }
            catch (Exception ex)
            {
                _cache.Remove($"PROVIDER_OTP_{leadDto.Phone}");
                return StatusCode(500, new { message = "An error occurred while sending the email." });
            }

            return Ok(new { message = "OTP sent successfully to your email." });
        }

        [HttpPost("register-doctor")]
        public async Task<IActionResult> RegisterDoctor([FromBody] RegisterDoctorDto request, [FromServices] practo_backend.Data.ApplicationDbContext context)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Otp))
            {
                return BadRequest(new { message = "Email and OTP are required." });
            }

            if (_cache.TryGetValue($"PROVIDER_OTP_{request.Email}", out string? storedOtp))
            {
                if (storedOtp == request.Otp)
                {
                    _cache.Remove($"PROVIDER_OTP_{request.Email}");
                    
                    // Check if email already exists
                    if (await context.Users.AnyAsync(u => u.Email == request.Email))
                    {
                        return BadRequest(new { message = "An account with this email already exists." });
                    }

                    // Create User
                    var nameParts = request.Name?.Split(' ') ?? new[] { "Doctor" };
                    var user = new practo_backend.Models.User
                    {
                        FirstName = nameParts[0],
                        LastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "",
                        Email = request.Email,
                        PasswordHash = practo_backend.Services.PasswordHasher.HashPassword(request.Password),
                        PhoneNumber = "",
                        Role = practo_backend.Models.UserRole.Doctor,
                        IsVerified = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    await context.Users.AddAsync(user);
                    await context.SaveChangesAsync();

                    // Create Doctor profile
                    var doctor = new practo_backend.Models.Doctor
                    {
                        UserId = user.Id,
                        SpecialtyId = request.SpecialtyId,
                        IsApproved = false,
                        RecommendationPercentage = 100, // starting value
                        ExperienceYears = 0
                    };

                    await context.Doctors.AddAsync(doctor);
                    await context.SaveChangesAsync();

                    return Ok(new { message = "Registration successful! Your profile is currently under review by the Practo Admin team." });
                }
                
                return BadRequest(new { message = "Invalid OTP. Please try again." });
            }

            return BadRequest(new { message = "OTP expired or not found. Please request a new one." });
        }
    }

    public class RegisterDoctorDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int SpecialtyId { get; set; }
        public string Otp { get; set; } = string.Empty;
    }
}
