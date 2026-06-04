namespace practo_backend.Services;

public interface IEmailService
{
    Task SendOtpAsync(string toEmail, string otp);
}
