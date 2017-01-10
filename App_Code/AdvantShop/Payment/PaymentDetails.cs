//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;

namespace AdvantShop.Payment
{
    [Serializable]
    public class PaymentDetails
    {
        public string CompanyName { get; set; }
        public string INN { get; set; }
        public string Phone { get; set; }
    }
}