using practo_backend.Models;

namespace practo_backend.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}
