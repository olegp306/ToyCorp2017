using System;
using AdvantShop.Orders;

namespace AdvantShop.Modules
{
    public class AbandonedCart
    {
        public Guid CustomerId { get; set; }

        public OrderConfirmationData OrderConfirmationData { get; set; }

        public DateTime LastUpdate { get; set; }

        public int SendingCount { get; set; }

        public DateTime? SendingDate { get; set; }
    }
}