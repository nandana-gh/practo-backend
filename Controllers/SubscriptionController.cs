using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practo_backend.Data;
using practo_backend.Models;
using practo_backend.Services;

namespace practo_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IPaymentService _paymentService;
    private readonly IConfiguration _configuration;

    public SubscriptionController(ApplicationDbContext context, IPaymentService paymentService, IConfiguration configuration)
    {
        _context = context;
        _paymentService = paymentService;
        _configuration = configuration;
    }

    [HttpGet("plans")]
    public async Task<IActionResult> GetPlans()
    {
        var plans = await _context.SubscriptionPlans.ToListAsync();
        return Ok(plans);
    }

    [Authorize]
    [HttpPost("purchase/{planId}")]
    public async Task<IActionResult> PurchaseSubscription(int planId)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var plan = await _context.SubscriptionPlans.FindAsync(planId);
        if (plan == null) return NotFound("Plan not found");

        var subscription = new UserSubscription
        {
            UserId = userId,
            SubscriptionPlanId = planId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(plan.DurationMonths),
            Status = SubscriptionStatus.Pending
        };

        _context.UserSubscriptions.Add(subscription);
        await _context.SaveChangesAsync();

        string receiptId = $"sub_{subscription.Id}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
        subscription.RazorpayOrderId = _paymentService.CreateOrder(plan.Price, receiptId);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Subscription order created",
            subscriptionId = subscription.Id,
            fee = plan.Price,
            razorpayOrderId = subscription.RazorpayOrderId,
            razorpayKeyId = _configuration["Razorpay:KeyId"] ?? "YOUR_RAZORPAY_KEY_ID"
        });
    }
}
