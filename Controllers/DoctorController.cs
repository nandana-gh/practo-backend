using Microsoft.AspNetCore.Mvc;
using practo_backend.DTOs;
using practo_backend.Services;

namespace practo_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DoctorProfileDto>> GetDoctorProfile(int id)
    {
        var profile = await _doctorService.GetDoctorProfileAsync(id);
        if (profile == null) return NotFound("Doctor profile not found.");

        return Ok(profile);
    }

    [HttpGet("{id}/availability")]
    public async Task<ActionResult<List<AppointmentSlotDto>>> GetDoctorAvailability(
        int id, 
        [FromQuery] int? clinicId, 
        [FromQuery] DateTime? startDate, 
        [FromQuery] int days = 3)
    {
        var start = startDate ?? DateTime.UtcNow.Date;
        var slots = await _doctorService.GetDoctorAvailabilityAsync(id, clinicId, start, days);
        return Ok(slots);
    }
}
