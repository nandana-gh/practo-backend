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
public class TeleconsultController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TeleconsultController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("{appointmentId}/chat-history")]
    public async Task<IActionResult> GetChatHistory(int appointmentId)
    {
        var messages = await _context.ChatMessages
            .Where(m => m.AppointmentId == appointmentId)
            .OrderBy(m => m.SentAt)
            .Select(m => new {
                m.Id,
                m.SenderId,
                m.Message,
                m.SentAt
            })
            .ToListAsync();

        return Ok(messages);
    }

    [HttpPost("prescription")]
    public async Task<IActionResult> IssuePrescription([FromBody] PrescriptionDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var doctorId)) return Unauthorized();

        // Verify the appointment belongs to this doctor
        var appointment = await _context.Appointments.FindAsync(dto.AppointmentId);
        if (appointment == null || appointment.DoctorId != doctorId)
            return Forbid();

        var prescription = new Prescription
        {
            AppointmentId = dto.AppointmentId,
            Medications = dto.Medications,
            Instructions = dto.Instructions,
            IssuedAt = DateTime.UtcNow
        };

        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Prescription issued successfully." });
    }
}

public class PrescriptionDto
{
    public int AppointmentId { get; set; }
    public string Medications { get; set; } = string.Empty;
    public string? Instructions { get; set; }
}
