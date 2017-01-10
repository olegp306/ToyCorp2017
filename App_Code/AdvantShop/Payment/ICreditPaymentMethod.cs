namespace AdvantShop.Payment
{
    public interface ICreditPaymentMethod
    {
        float MinimumPrice { get; set; }
        float FirstPayment { get; set; }
        int PaymentMethodId { get; }
    }
}