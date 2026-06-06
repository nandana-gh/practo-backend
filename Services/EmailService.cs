using System.IO;
using System.Net;
using System.Net.Mail;

namespace practo_backend.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _config;
    private readonly string _scratchPath = @"C:\Users\nandana.r\.gemini\antigravity-ide\brain\96278cca-e055-4a67-8f80-c611d47f487a\scratch";

    public EmailService(ILogger<EmailService> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    public async Task SendOtpAsync(string toEmail, string otp)
    {
        var smtpSettings = _config.GetSection("Smtp");
        var host = smtpSettings["Host"];
        var username = smtpSettings["Username"];
        
        // If the user hasn't put in their real email yet, fallback to local log simulation
        if (string.IsNullOrEmpty(host) || username == "your_email@gmail.com")
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
            return;
        }

        // Real SMTP configuration
        var port = int.Parse(smtpSettings["Port"] ?? "587");
        var password = smtpSettings["Password"];
        var enableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true");

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = enableSsl
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(username!, "Practo Clone"),
            Subject = "Your Practo Verification OTP",
            Body = $"<h2>Welcome to Practo!</h2><p>Your OTP for email verification is: <b style='font-size:20px; color:#2CB7DF;'>{otp}</b></p><p>It is valid for 10 minutes.</p>",
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        try
        {
            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Successfully sent real OTP email to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send real OTP email to {Email}", toEmail);
            throw; 
        }
    }
}
