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

    [HttpGet("{id}/patients")]
    public async Task<ActionResult<List<DoctorPatientDto>>> GetDoctorPatients(int id)
    {
        var patients = await _doctorService.GetDoctorPatientsAsync(id);
        return Ok(patients);
    }

    [HttpGet("{id}/reports")]
    public async Task<ActionResult<DoctorReportDto>> GetDoctorReports(int id)
    {
        var reports = await _doctorService.GetDoctorReportsAsync(id);
        return Ok(reports);
    }

    [HttpPut("{id}/profile")]
    public async Task<IActionResult> UpdateDoctorProfile(int id, [FromBody] DoctorProfileUpdateDto dto)
    {
        var result = await _doctorService.UpdateDoctorProfileAsync(id, dto);
        if (!result) return NotFound("Doctor not found.");
        return Ok(new { message = "Profile updated successfully." });
    }
    [HttpGet("user/{userId}/profile")]
    public async Task<ActionResult<DoctorProfileDto>> GetDoctorProfileByUser(int userId)
    {
        var doctorId = await _doctorService.GetDoctorIdByUserIdAsync(userId);
        if (doctorId == null) return NotFound("Doctor not found.");

        var profile = await _doctorService.GetDoctorProfileAsync(doctorId.Value);
        if (profile == null) return NotFound("Doctor profile not found.");

        return Ok(profile);
    }

    [HttpGet("user/{userId}/availability")]
    public async Task<ActionResult<List<AppointmentSlotDto>>> GetDoctorAvailabilityByUser(
        int userId, 
        [FromQuery] int? clinicId, 
        [FromQuery] DateTime? startDate, 
        [FromQuery] int days = 3)
    {
        var doctorId = await _doctorService.GetDoctorIdByUserIdAsync(userId);
        if (doctorId == null) return NotFound("Doctor not found.");

        var start = startDate ?? DateTime.UtcNow.Date;
        var slots = await _doctorService.GetDoctorAvailabilityAsync(doctorId.Value, clinicId, start, days);
        return Ok(slots);
    }

    [HttpGet("user/{userId}/patients")]
    public async Task<ActionResult<List<DoctorPatientDto>>> GetDoctorPatientsByUser(int userId)
    {
        var doctorId = await _doctorService.GetDoctorIdByUserIdAsync(userId);
        if (doctorId == null) return NotFound("Doctor not found.");

        var patients = await _doctorService.GetDoctorPatientsAsync(doctorId.Value);
        return Ok(patients);
    }

    [HttpGet("user/{userId}/reports")]
    public async Task<ActionResult<DoctorReportDto>> GetDoctorReportsByUser(int userId)
    {
        var doctorId = await _doctorService.GetDoctorIdByUserIdAsync(userId);
        if (doctorId == null) return NotFound("Doctor not found.");

        var reports = await _doctorService.GetDoctorReportsAsync(doctorId.Value);
        return Ok(reports);
    }

    [HttpPut("user/{userId}/profile")]
    public async Task<IActionResult> UpdateDoctorProfileByUser(int userId, [FromBody] DoctorProfileUpdateDto dto)
    {
        var doctorId = await _doctorService.GetDoctorIdByUserIdAsync(userId);
        if (doctorId == null) return NotFound("Doctor not found.");

        var result = await _doctorService.UpdateDoctorProfileAsync(doctorId.Value, dto);
        if (!result) return NotFound("Doctor not found.");
        return Ok(new { message = "Profile updated successfully." });
    }

    [HttpPost("user/{userId}/clinics")]
    public async Task<IActionResult> AddClinicToDoctorByUser(int userId, [FromBody] DoctorClinicCreateDto dto)
    {
        var doctorId = await _doctorService.GetDoctorIdByUserIdAsync(userId);
        if (doctorId == null) return NotFound("Doctor not found.");

        var result = await _doctorService.AddClinicToDoctorAsync(doctorId.Value, dto);
        if (!result) return NotFound("Doctor not found.");
        return Ok(new { message = "Clinic added successfully." });
    }
}
