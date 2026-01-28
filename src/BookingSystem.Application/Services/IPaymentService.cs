namespace BookingSystem.Application.Services;

public interface IPaymentService
{
    bool AddPaymentCard(string cardNumber, string cardHolderName, string expiryDate, string cvv);
    bool PaymentCharge(decimal amount, string cardNumber, string cardHolderName, string expiryDate, string cvv);
}
