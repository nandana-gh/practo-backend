using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practo_backend.Data;
using practo_backend.Models;

namespace practo_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MedicalRecordsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MedicalRecordsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("my-records")]
    public async Task<IActionResult> GetMyRecords()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var patientId)) return Unauthorized();

        var records = await _context.MedicalRecords
            .Where(r => r.PatientId == patientId)
            .OrderByDescending(r => r.UploadedAt)
            .Select(r => new {
                r.Id,
                r.Description,
                r.FileUrl,
                r.UploadedAt
            })
            .ToListAsync();

        return Ok(records);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadRecord([FromBody] MedicalRecordUploadDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var patientId)) return Unauthorized();

        var record = new MedicalRecord
        {
            PatientId = patientId,
            Description = dto.Description,
            FileUrl = dto.FileUrl, // In a real app, you would handle IFormFile and Azure Blob/S3
            UploadedAt = DateTime.UtcNow
        };

        _context.MedicalRecords.Add(record);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Record uploaded successfully", recordId = record.Id });
    }

    [HttpGet("my-prescriptions")]
    public async Task<IActionResult> GetMyPrescriptions()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var patientId)) return Unauthorized();

        var prescriptions = await _context.Prescriptions
            .Include(p => p.Appointment)
            .ThenInclude(a => a.Doctor)
            .ThenInclude(d => d.User)
            .Where(p => p.Appointment.PatientId == patientId)
            .OrderByDescending(p => p.IssuedAt)
            .Select(p => new {
                p.Id,
                p.AppointmentId,
                DoctorName = p.Appointment.Doctor.User.FirstName + " " + p.Appointment.Doctor.User.LastName,
                p.Medications,
                p.Instructions,
                p.IssuedAt
            })
            .ToListAsync();

        return Ok(prescriptions);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetPatientRecords(int patientId)
    {
        var records = await _context.MedicalRecords
            .Where(r => r.PatientId == patientId)
            .OrderByDescending(r => r.UploadedAt)
            .Select(r => new {
                r.Id,
                r.Description,
                r.FileUrl,
                r.UploadedAt
            })
            .ToListAsync();

        return Ok(records);
    }
}

public class MedicalRecordUploadDto
{
    public string Description { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
}
