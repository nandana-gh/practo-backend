using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practo_backend.Data;
using practo_backend.Models;
using practo_backend.Services;
using System.Security.Claims;

namespace practo_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ApplicationDbContext _context;

    public PaymentController(IPaymentService paymentService, ApplicationDbContext context)
    {
        _paymentService = paymentService;
        _context = context;
    }

    public class PaymentVerificationDto
    {
        public string RazorpayOrderId { get; set; } = string.Empty;
        public string RazorpayPaymentId { get; set; } = string.Empty;
        public string RazorpaySignature { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "Appointment", "Subscription", "Medicine"
        public int ReferenceId { get; set; } // The ID of the Appointment, UserSubscription, or MedicineOrder
    }

    public class CartItemDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty; // "Medicine" or "LabTest"
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class CartCheckoutDto
    {
        public List<CartItemDto> Items { get; set; } = new();
        public string ShippingAddress { get; set; } = string.Empty;
    }

    public class CartVerificationDto
    {
        public string RazorpayOrderId { get; set; } = string.Empty;
        public string RazorpayPaymentId { get; set; } = string.Empty;
        public string RazorpaySignature { get; set; } = string.Empty;
        public string OrderGroupToken { get; set; } = string.Empty;
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyPayment([FromBody] PaymentVerificationDto dto)
    {
        bool isValid = _paymentService.VerifySignature(dto.RazorpayOrderId, dto.RazorpayPaymentId, dto.RazorpaySignature);

        if (!isValid)
        {
            return BadRequest(new { message = "Payment verification failed. Invalid signature." });
        }

        if (dto.Type == "Appointment")
        {
            var appointment = await _context.Appointments.FindAsync(dto.ReferenceId);
            if (appointment == null) return NotFound("Appointment not found");

            appointment.Status = AppointmentStatus.Confirmed;
            appointment.RazorpayPaymentId = dto.RazorpayPaymentId;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Appointment confirmed successfully." });
        }
        else if (dto.Type == "Subscription")
        {
            var sub = await _context.UserSubscriptions.FindAsync(dto.ReferenceId);
            if (sub == null) return NotFound("Subscription not found");

            sub.Status = SubscriptionStatus.Active;
            sub.RazorpayPaymentId = dto.RazorpayPaymentId;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Subscription activated successfully." });
        }
        else if (dto.Type == "Medicine")
        {
            var order = await _context.MedicineOrders.FindAsync(dto.ReferenceId);
            if (order == null) return NotFound("Medicine order not found");

            order.Status = OrderStatus.Paid;
            order.RazorpayPaymentId = dto.RazorpayPaymentId;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Medicine order paid successfully." });
        }

        return BadRequest("Invalid payment type.");
    }

    [HttpPost("checkout-cart")]
    public async Task<IActionResult> CheckoutCart([FromBody] CartCheckoutDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized();

        if (dto.Items.Count == 0) return BadRequest("Cart is empty");

        decimal totalAmount = 0;
        
        var medicineItems = dto.Items.Where(i => i.Type == "Medicine").ToList();
        var labTestItems = dto.Items.Where(i => i.Type == "LabTest").ToList();

        MedicineOrder? medOrder = null;
        if (medicineItems.Any())
        {
            decimal medTotal = 0;
            var orderItems = new List<MedicineOrderItem>();
            foreach (var item in medicineItems)
            {
                var product = await _context.MedicineProducts.FindAsync(item.Id);
                if (product != null)
                {
                    medTotal += product.Price * item.Quantity;
                    orderItems.Add(new MedicineOrderItem { MedicineProductId = product.Id, Quantity = item.Quantity, UnitPrice = product.Price });
                }
            }
            if (medTotal > 0)
            {
                medOrder = new MedicineOrder
                {
                    UserId = userId, TotalAmount = medTotal, ShippingAddress = dto.ShippingAddress, Status = OrderStatus.Pending, OrderDate = DateTime.UtcNow, Items = orderItems
                };
                _context.MedicineOrders.Add(medOrder);
                totalAmount += medTotal;
            }
        }

        LabTestOrder? labOrder = null;
        if (labTestItems.Any())
        {
            decimal labTotal = 0;
            var orderItems = new List<LabTestOrderItem>();
            foreach (var item in labTestItems)
            {
                labTotal += item.Price * item.Quantity;
                orderItems.Add(new LabTestOrderItem { Quantity = item.Quantity, UnitPrice = item.Price });
            }
            if (labTotal > 0)
            {
                labOrder = new LabTestOrder
                {
                    UserId = userId, TotalAmount = labTotal, ShippingAddress = dto.ShippingAddress, Status = OrderStatus.Pending, OrderDate = DateTime.UtcNow, Items = orderItems
                };
                _context.LabTestOrders.Add(labOrder);
                totalAmount += labTotal;
            }
        }

        if (totalAmount == 0) return BadRequest("Invalid items");

        await _context.SaveChangesAsync();

        string orderGroupToken = Guid.NewGuid().ToString("N");
        string receiptId = $"cart_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
        string rzpOrderId = _paymentService.CreateOrder(totalAmount, receiptId);

        if (medOrder != null) { medOrder.RazorpayOrderId = rzpOrderId; }
        if (labOrder != null) { labOrder.RazorpayOrderId = rzpOrderId; }
        await _context.SaveChangesAsync();

        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        return Ok(new
        {
            message = "Cart order created",
            fee = totalAmount,
            razorpayOrderId = rzpOrderId,
            razorpayKeyId = config["Razorpay:KeyId"],
            orderGroupToken = orderGroupToken
        });
    }

    [HttpPost("verify-cart")]
    public async Task<IActionResult> VerifyCartPayment([FromBody] CartVerificationDto dto)
    {
        bool isValid = _paymentService.VerifySignature(dto.RazorpayOrderId, dto.RazorpayPaymentId, dto.RazorpaySignature);
        if (!isValid) return BadRequest("Invalid signature");

        var medOrders = await _context.MedicineOrders.Where(m => m.RazorpayOrderId == dto.RazorpayOrderId).ToListAsync();
        foreach (var m in medOrders) { m.Status = OrderStatus.Paid; m.RazorpayPaymentId = dto.RazorpayPaymentId; }

        var labOrders = await _context.LabTestOrders.Where(l => l.RazorpayOrderId == dto.RazorpayOrderId).ToListAsync();
        foreach (var l in labOrders) { l.Status = OrderStatus.Paid; l.RazorpayPaymentId = dto.RazorpayPaymentId; }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Cart payment successful" });
    }
}
