namespace practo_backend.Services;

public interface IEmailService
{
    Task SendOtpAsync(string toEmail, string otp);
    Task SendSurgeryNotificationAsync(string toEmail, string name, string surgeryName);
}
