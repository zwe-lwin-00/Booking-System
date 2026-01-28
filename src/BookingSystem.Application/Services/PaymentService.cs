namespace BookingSystem.Application.Services;

public class PaymentService : IPaymentService
{
    public bool AddPaymentCard(string cardNumber, string cardHolderName, string expiryDate, string cvv)
    {
        // Mock payment service - in production, integrate with real payment gateway
        // Validate card details
        if (string.IsNullOrWhiteSpace(cardNumber) || 
            string.IsNullOrWhiteSpace(cardHolderName) ||
            string.IsNullOrWhiteSpace(expiryDate) ||
            string.IsNullOrWhiteSpace(cvv))
        {
            throw new ArgumentException("All payment card details are required");
        }

        // Simulate payment card validation
        return true;
    }

    public bool PaymentCharge(decimal amount, string cardNumber, string cardHolderName, string expiryDate, string cvv)
    {
        // Mock payment service
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero");

        if (string.IsNullOrWhiteSpace(cardNumber) || 
            string.IsNullOrWhiteSpace(cardHolderName) ||
            string.IsNullOrWhiteSpace(expiryDate) ||
            string.IsNullOrWhiteSpace(cvv))
        {
            throw new ArgumentException("All payment card details are required");
        }

        // Simulate payment processing
        // In production: Call actual payment gateway API
        return true;
    }
}
