using Microsoft.AspNetCore.Mvc;
using practo_backend.DTOs;
using practo_backend.Services;

namespace practo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LabTestsController : ControllerBase
    {
        private readonly ILabTestService _labTestService;

        public LabTestsController(ILabTestService labTestService)
        {
            _labTestService = labTestService;
        }

        [HttpGet("landing")]
        public async Task<ActionResult<LabTestsLandingDto>> GetLandingData()
        {
            var data = await _labTestService.GetLandingDataAsync();
            return Ok(data);
        }
    }
}
