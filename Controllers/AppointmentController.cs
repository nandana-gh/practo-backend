using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using practo_backend.DTOs;
using practo_backend.Services;

namespace practo_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpPost("book")]
    public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var patientId))
        {
            return Unauthorized("Invalid user token.");
        }

        try
        {
            var appointment = await _appointmentService.BookAppointmentAsync(patientId, dto);
            if (appointment == null)
            {
                return Conflict(new { message = "The selected time slot is no longer available." });
            }

            // In a real flow, this would return an order ID for Razorpay.
            return Ok(new { message = "Appointment booked successfully", appointmentId = appointment.Id, fee = appointment.Fee });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while booking the appointment.", details = ex.Message });
        }
    }
}
