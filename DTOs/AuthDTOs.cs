using practo_backend.Models;

namespace practo_backend.DTOs;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string PhoneNumber,
    UserRole Role,
    int? ExperienceYears = null,
    decimal? VideoConsultationFee = null
);

public record LoginRequest(
    string Email,
    string Password
);

public record VerifyOtpRequest(
    string Email,
    string OtpCode
);

public record OtpRequestDto(
    string Email
);

public record AuthResponse(
    string Token,
    int UserId,
    string Email,
    string FirstName,
    string LastName,
    string Role
);
