using Microsoft.Extensions.Logging;

namespace BookingSystem.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(ILogger<PaymentService> logger)
    {
        _logger = logger;
    }

    public bool AddPaymentCard(string cardNumber, string cardHolderName, string expiryDate, string cvv)
    {
        _logger.LogInformation("Add payment card (mock) for holder {CardHolderName}", cardHolderName);
        // Mock payment service - in production, integrate with real payment gateway
        // Validate card details
        if (string.IsNullOrWhiteSpace(cardNumber) || 
            string.IsNullOrWhiteSpace(cardHolderName) ||
            string.IsNullOrWhiteSpace(expiryDate) ||
            string.IsNullOrWhiteSpace(cvv))
        {
            _logger.LogWarning("Add payment card failed: missing card details");
            throw new ArgumentException("All payment card details are required");
        }

        // Simulate payment card validation
        return true;
    }

    public bool PaymentCharge(decimal amount, string cardNumber, string cardHolderName, string expiryDate, string cvv)
    {
        _logger.LogInformation("Payment charge (mock) amount {Amount} for holder {CardHolderName}", amount, cardHolderName);
        // Mock payment service
        if (amount <= 0)
        {
            _logger.LogWarning("Payment charge failed: amount must be greater than zero");
            throw new ArgumentException("Amount must be greater than zero");
        }

        if (string.IsNullOrWhiteSpace(cardNumber) || 
            string.IsNullOrWhiteSpace(cardHolderName) ||
            string.IsNullOrWhiteSpace(expiryDate) ||
            string.IsNullOrWhiteSpace(cvv))
        {
            _logger.LogWarning("Payment charge failed: missing card details");
            throw new ArgumentException("All payment card details are required");
        }

        // Simulate payment processing
        // In production: Call actual payment gateway API
        return true;
    }
}
