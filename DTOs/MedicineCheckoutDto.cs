namespace practo_backend.DTOs;

public class MedicineCheckoutDto
{
    public string ShippingAddress { get; set; } = string.Empty;
    public List<CartItemDto> Items { get; set; } = new();
}

public class CartItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
