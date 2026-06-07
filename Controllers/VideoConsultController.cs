using Microsoft.AspNetCore.Mvc;
using practo_backend.DTOs;
using practo_backend.Services;

namespace practo_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VideoConsultController : ControllerBase
{
    private readonly IVideoConsultService _videoConsultService;

    public VideoConsultController(IVideoConsultService videoConsultService)
    {
        _videoConsultService = videoConsultService;
    }

    [HttpGet("landing")]
    public async Task<ActionResult<VideoConsultLandingDto>> GetLandingPageData()
    {
        var data = await _videoConsultService.GetLandingPageDataAsync();
        return Ok(data);
    }
}
