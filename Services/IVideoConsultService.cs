using practo_backend.DTOs;

namespace practo_backend.Services;

public interface IVideoConsultService
{
    Task<VideoConsultLandingDto> GetLandingPageDataAsync();
}
