namespace practo_backend.Models;

public enum OrderStatus
{
    Pending,
    Paid,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}

public class MedicineOrder
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    
    public string ShippingAddress { get; set; } = string.Empty;
    
    public string RazorpayOrderId { get; set; } = string.Empty;
    public string RazorpayPaymentId { get; set; } = string.Empty;
    
    public List<MedicineOrderItem> Items { get; set; } = new();
}

public class MedicineOrderItem
{
    public int Id { get; set; }
    public int MedicineOrderId { get; set; }
    public MedicineOrder? MedicineOrder { get; set; }
    
    public int MedicineProductId { get; set; }
    public MedicineProduct? MedicineProduct { get; set; }
    
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
