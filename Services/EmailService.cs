using System.IO;

namespace practo_backend.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly string _scratchPath = @"C:\Users\nandana.r\.gemini\antigravity-ide\brain\96278cca-e055-4a67-8f80-c611d47f487a\scratch";

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendOtpAsync(string toEmail, string otp)
    {
        var message = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] OTP for {toEmail} is: {otp}\n";
        _logger.LogInformation("EMAIL SIMULATOR: Send OTP {Otp} to {Email}", otp, toEmail);

        try
        {
            if (!Directory.Exists(_scratchPath))
            {
                Directory.CreateDirectory(_scratchPath);
            }
            var filePath = Path.Combine(_scratchPath, "otp_logs.txt");
            await File.AppendAllTextAsync(filePath, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write OTP to scratch file");
        }
    }
}
