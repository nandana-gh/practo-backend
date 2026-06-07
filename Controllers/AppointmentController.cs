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
    private readonly IConfiguration _configuration;

    public AppointmentController(IAppointmentService appointmentService, IConfiguration configuration)
    {
        _appointmentService = appointmentService;
        _configuration = configuration;
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

            return Ok(new { 
                message = "Appointment booked successfully", 
                appointmentId = appointment.Id, 
                fee = appointment.Fee,
                razorpayOrderId = appointment.RazorpayOrderId,
                razorpayKeyId = _configuration["Razorpay:KeyId"] ?? "YOUR_RAZORPAY_KEY_ID"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while booking the appointment.", details = ex.Message });
        }
    }

    [HttpGet("patient")]
    public async Task<IActionResult> GetPatientAppointments()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var patientId))
        {
            return Unauthorized("Invalid user token.");
        }

        var appointments = await _appointmentService.GetPatientAppointmentsAsync(patientId);
        return Ok(appointments);
    }

    [HttpGet("doctor")]
    public async Task<IActionResult> GetDoctorAppointments()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var doctorId))
        {
            return Unauthorized("Invalid user token.");
        }

        var appointments = await _appointmentService.GetDoctorAppointmentsAsync(doctorId);
        return Ok(appointments);
    }
}
