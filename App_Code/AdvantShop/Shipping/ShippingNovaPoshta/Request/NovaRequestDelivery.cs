using System;

namespace AdvantShop.Shipping
{
    public class NovaRequestDelivery
    {
        public Guid CitySender { get; set; }
        public Guid CityRecipient { get; set; }
        public string ServiceType { get; set; }
        public string DateTime { get; set; }
    }
}