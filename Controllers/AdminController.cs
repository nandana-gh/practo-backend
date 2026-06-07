using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practo_backend.Data;
using practo_backend.Models;
using practo_backend.Services;

namespace PractoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]

    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public AdminController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet("pending-doctors")]
        public async Task<IActionResult> GetPendingDoctors()
        {
            var pendingDoctors = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialty)
                .Where(d => !d.IsApproved)
                .Select(d => new
                {
                    d.Id,
                    Name = d.User.FirstName + " " + d.User.LastName,
                    d.User.Email,
                    d.User.PhoneNumber,
                    Specialty = d.Specialty.Name,
                    d.User.CreatedAt
                })
                .ToListAsync();

            return Ok(pendingDoctors);
        }

        [HttpPost("approve-doctor/{id}")]
        public async Task<IActionResult> ApproveDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound(new { message = "Doctor not found." });
            }

            if (doctor.IsApproved)
            {
                return BadRequest(new { message = "Doctor is already approved." });
            }

            doctor.IsApproved = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Doctor approved successfully. They are now live on the platform." });
        }

        [HttpGet("medicine-orders")]
        public async Task<IActionResult> GetMedicineOrders()
        {
            var orders = await _context.MedicineOrders
                .Include(o => o.Items)
                .ThenInclude(i => i.MedicineProduct)
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(orders.Select(o => new
            {
                o.Id,
                o.TotalAmount,
                o.OrderDate,
                Status = o.Status.ToString(),
                o.ShippingAddress,
                PatientName = o.User?.FirstName + " " + o.User?.LastName,
                PatientEmail = o.User?.Email,
                o.RazorpayOrderId,
                o.RazorpayPaymentId,
                Items = o.Items.Select(i => new
                {
                    i.MedicineProduct?.Name,
                    i.Quantity,
                    i.UnitPrice
                })
            }));
        }

        [HttpGet("labtest-orders")]
        public async Task<IActionResult> GetLabTestOrders()
        {
            var orders = await _context.LabTestOrders
                .Include(o => o.Items)
                    .ThenInclude(i => i.DiagnosticTest)
                .Include(o => o.Items)
                    .ThenInclude(i => i.HealthCheckupPackage)
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(orders.Select(o => new
            {
                o.Id,
                o.TotalAmount,
                o.OrderDate,
                Status = o.Status.ToString(),
                PatientName = o.User?.FirstName + " " + o.User?.LastName,
                PatientEmail = o.User?.Email,
                o.RazorpayOrderId,
                o.RazorpayPaymentId,
                Items = o.Items.Select(i => new
                {
                    TestName = i.DiagnosticTest != null ? i.DiagnosticTest.Name : (i.HealthCheckupPackage != null ? i.HealthCheckupPackage.Name : "Unknown"),
                    Price = i.UnitPrice
                })
            }));
        }

        [HttpGet("surgery-leads")]
        public async Task<IActionResult> GetSurgeryLeads()
        {
            var leads = await _context.SurgeryLeads
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => new
                {
                    l.Id,
                    l.Name,
                    l.MobileNumber,
                    l.City,
                    l.SurgeryName,
                    l.CreatedAt
                })
                .ToListAsync();

            return Ok(leads);
        }

        [HttpPost("intimate-surgery-lead/{id}")]
        public async Task<IActionResult> IntimateSurgeryLead(int id)
        {
            var lead = await _context.SurgeryLeads.FindAsync(id);
            if (lead == null)
            {
                return NotFound(new { Message = "Lead not found." });
            }

            if (string.IsNullOrEmpty(lead.Email))
            {
                return BadRequest(new { Message = "Patient email is missing. Cannot send notification." });
            }

            await _emailService.SendSurgeryNotificationAsync(lead.Email, lead.Name, lead.SurgeryName);

            return Ok(new { Message = "Notification successfully sent to the patient." });
        }

        [HttpGet("patients")]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await _context.Users
                .Where(u => u.Role == UserRole.Patient)
                .Select(u => new
                {
                    u.Id,
                    Name = u.FirstName + " " + u.LastName,
                    u.Email,
                    u.PhoneNumber,
                    u.CreatedAt,
                    u.IsVerified
                })
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            return Ok(patients);
        }

        [HttpGet("doctors")]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialty)
                .Select(d => new
                {
                    d.Id,
                    d.UserId,
                    Name = d.User.FirstName + " " + d.User.LastName,
                    d.User.Email,
                    d.User.PhoneNumber,
                    Specialty = d.Specialty.Name,
                    d.ExperienceYears,
                    ConsultationFee = d.VideoConsultationFee,
                    d.IsApproved,
                    d.User.CreatedAt
                })
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return Ok(doctors);
        }

        [HttpGet("appointments")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
                .Select(a => new
                {
                    a.Id,
                    PatientName = a.Patient.FirstName + " " + a.Patient.LastName,
                    DoctorName = a.Doctor.User.FirstName + " " + a.Doctor.User.LastName,
                    a.AppointmentDateTime,
                    a.Type,
                    a.Status,
                    Amount = a.Fee,
                    a.ReasonForVisit
                })
                .OrderByDescending(a => a.AppointmentDateTime)
                .ToListAsync();

            return Ok(appointments);
        }

        // --- INVENTORY MANAGEMENT: MEDICINES ---

        [HttpGet("inventory/medicines")]
        public async Task<IActionResult> GetMedicineInventory()
        {
            var medicines = await _context.MedicineProducts.ToListAsync();
            return Ok(medicines);
        }

        [HttpPost("inventory/medicines")]
        public async Task<IActionResult> AddMedicine([FromBody] MedicineProduct medicine)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.MedicineProducts.Add(medicine);
            await _context.SaveChangesAsync();
            return Ok(medicine);
        }

        [HttpDelete("inventory/medicines/{id}")]
        public async Task<IActionResult> DeleteMedicine(int id)
        {
            var medicine = await _context.MedicineProducts.FindAsync(id);
            if (medicine == null)
                return NotFound();

            _context.MedicineProducts.Remove(medicine);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Medicine deleted successfully" });
        }

        // --- INVENTORY MANAGEMENT: LAB TESTS ---

        [HttpGet("inventory/labtests")]
        public async Task<IActionResult> GetLabTestInventory()
        {
            var tests = await _context.DiagnosticTests.ToListAsync();
            return Ok(tests);
        }

        [HttpPost("inventory/labtests")]
        public async Task<IActionResult> AddLabTest([FromBody] DiagnosticTest test)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.DiagnosticTests.Add(test);
            await _context.SaveChangesAsync();
            return Ok(test);
        }

        [HttpDelete("inventory/labtests/{id}")]
        public async Task<IActionResult> DeleteLabTest(int id)
        {
            var test = await _context.DiagnosticTests.FindAsync(id);
            if (test == null)
                return NotFound();

            _context.DiagnosticTests.Remove(test);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Lab test deleted successfully" });
        }
    }
}
