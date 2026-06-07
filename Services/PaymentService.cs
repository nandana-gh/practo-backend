using Microsoft.Extensions.Configuration;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;

namespace practo_backend.Services;

public class PaymentService : IPaymentService
{
    private readonly string _keyId;
    private readonly string _keySecret;

    public PaymentService(IConfiguration configuration)
    {
        _keyId = configuration["Razorpay:KeyId"] ?? "YOUR_RAZORPAY_KEY_ID";
        _keySecret = configuration["Razorpay:KeySecret"] ?? "YOUR_RAZORPAY_KEY_SECRET";
    }

    public string CreateOrder(decimal amount, string receiptId)
    {
        if (_keyId == "YOUR_RAZORPAY_KEY_ID")
        {
            // Simulate Razorpay order creation for dummy keys
            return "order_dummy_" + Guid.NewGuid().ToString().Substring(0, 8);
        }

        try
        {
            RazorpayClient client = new RazorpayClient(_keyId, _keySecret);
            
            Dictionary<string, object> options = new Dictionary<string, object>
            {
                { "amount", (int)(amount * 100) }, // amount in the smallest currency unit
                { "currency", "INR" },
                { "receipt", receiptId },
                { "payment_capture", 1 }
            };

            Order order = client.Order.Create(options);
            return order["id"].ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Razorpay Error: {ex.Message}");
            throw;
        }
    }

    public bool VerifySignature(string orderId, string paymentId, string signature)
    {
        if (_keyId == "YOUR_RAZORPAY_KEY_ID")
        {
            // Simulate successful verification for dummy keys
            return true;
        }

        try
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            attributes.Add("razorpay_payment_id", paymentId);
            attributes.Add("razorpay_order_id", orderId);
            attributes.Add("razorpay_signature", signature);

            Utils.verifyPaymentSignature(attributes);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
