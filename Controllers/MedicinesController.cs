using Microsoft.AspNetCore.Mvc;
using practo_backend.DTOs;
using practo_backend.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using practo_backend.Data;
using practo_backend.Models;
using System.Security.Claims;

namespace practo_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicinesController : ControllerBase
{
    private readonly IMedicineService _medicineService;
    private readonly ApplicationDbContext _context;
    private readonly IPaymentService _paymentService;
    private readonly IConfiguration _configuration;

    public MedicinesController(IMedicineService medicineService, ApplicationDbContext context, IPaymentService paymentService, IConfiguration configuration)
    {
        _medicineService = medicineService;
        _context = context;
        _paymentService = paymentService;
        _configuration = configuration;
    }

    [HttpGet("landing")]
    public async Task<ActionResult<MedicinesLandingDto>> GetLandingData()
    {
        try
        {
            var data = await _medicineService.GetLandingDataAsync();
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving medicines landing data.", error = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] MedicineCheckoutDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized();

        if (dto.Items.Count == 0) return BadRequest("Cart is empty");

        decimal totalAmount = 0;
        var orderItems = new List<MedicineOrderItem>();

        foreach (var item in dto.Items)
        {
            var product = await _context.MedicineProducts.FindAsync(item.ProductId);
            if (product == null) continue;

            var itemTotal = product.Price * item.Quantity;
            totalAmount += itemTotal;

            orderItems.Add(new MedicineOrderItem
            {
                MedicineProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });
        }

        if (totalAmount == 0) return BadRequest("Invalid items");

        var order = new MedicineOrder
        {
            UserId = userId,
            TotalAmount = totalAmount,
            ShippingAddress = dto.ShippingAddress,
            Status = OrderStatus.Pending,
            OrderDate = DateTime.UtcNow,
            Items = orderItems
        };

        _context.MedicineOrders.Add(order);
        await _context.SaveChangesAsync();

        string receiptId = $"med_{order.Id}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
        order.RazorpayOrderId = _paymentService.CreateOrder(totalAmount, receiptId);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Order created successfully",
            orderId = order.Id,
            fee = totalAmount,
            razorpayOrderId = order.RazorpayOrderId,
            razorpayKeyId = _configuration["Razorpay:KeyId"] ?? "YOUR_RAZORPAY_KEY_ID"
        });
    }

    [Authorize]
    [HttpGet("orders")]
    public async Task<IActionResult> GetMyOrders()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var orders = await _context.MedicineOrders
            .Include(o => o.Items)
            .ThenInclude(i => i.MedicineProduct)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return Ok(orders.Select(o => new
        {
            o.Id,
            o.TotalAmount,
            o.OrderDate,
            Status = o.Status.ToString(),
            o.ShippingAddress,
            Items = o.Items.Select(i => new
            {
                i.MedicineProduct?.Name,
                i.Quantity,
                i.UnitPrice
            })
        }));
    }
}
