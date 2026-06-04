using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practo_backend.Data;
using practo_backend.DTOs;
using practo_backend.Models;
using practo_backend.Services;

namespace practo_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;

    public AuthController(
        ApplicationDbContext context,
        ITokenService tokenService,
        IEmailService emailService)
    {
        _context = context;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest(new { Message = "Email is already registered" });
        }

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = PasswordHasher.HashPassword(request.Password),
            PhoneNumber = request.PhoneNumber,
            Role = request.Role,
            IsVerified = false
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Automatically trigger OTP send for verification
        await SendOtpInternalAsync(user.Email);

        return Ok(new { Message = "Registration successful. Please verify the OTP sent to your email." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { Message = "Invalid email or password" });
        }

        if (!user.IsVerified)
        {
            // If not verified, trigger a new OTP
            await SendOtpInternalAsync(user.Email);
            return StatusCode(StatusCodes.Status403Forbidden, new { 
                Message = "Account email not verified. An OTP has been sent to your email.",
                RequiresVerification = true,
                Email = user.Email
            });
        }

        var token = _tokenService.GenerateToken(user);
        return Ok(new AuthResponse(
            Token: token,
            UserId: user.Id,
            Email: user.Email,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Role: user.Role.ToString()
        ));
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] OtpRequestDto request)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Email == request.Email);
        if (!userExists)
        {
            return NotFound(new { Message = "User with this email not found" });
        }

        await SendOtpInternalAsync(request.Email);
        return Ok(new { Message = "OTP sent successfully" });
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        var otpRecord = await _context.OtpRequests
            .Where(o => o.Email == request.Email && !o.IsUsed)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (otpRecord == null || otpRecord.OtpCode != request.OtpCode || otpRecord.ExpiryTime < DateTime.UtcNow)
        {
            return BadRequest(new { Message = "Invalid or expired OTP" });
        }

        otpRecord.IsUsed = true;
        _context.OtpRequests.Update(otpRecord);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user != null)
        {
            user.IsVerified = true;
            _context.Users.Update(user);
        }

        await _context.SaveChangesAsync();

        if (user == null)
        {
            return NotFound(new { Message = "Associated user not found" });
        }

        var token = _tokenService.GenerateToken(user);
        return Ok(new AuthResponse(
            Token: token,
            UserId: user.Id,
            Email: user.Email,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Role: user.Role.ToString()
        ));
    }

    private async Task SendOtpInternalAsync(string email)
    {
        var otp = new Random().Next(100000, 999999).ToString();
        var otpRecord = new OtpRequest
        {
            Email = email,
            OtpCode = otp,
            ExpiryTime = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false
        };

        _context.OtpRequests.Add(otpRecord);
        await _context.SaveChangesAsync();

        await _emailService.SendOtpAsync(email, otp);
    }
}
