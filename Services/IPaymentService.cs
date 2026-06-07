namespace practo_backend.Services;

public interface IPaymentService
{
    string CreateOrder(decimal amount, string receiptId);
    bool VerifySignature(string orderId, string paymentId, string signature);
}
