//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;

namespace AdvantShop.Payment
{
    public class PaymentGiftCertificate : PaymentMethod
    {
        public override PaymentType Type
        {
            get { return PaymentType.GiftCertificate; }
        }
        public override ProcessType ProcessType
        {
            get { return ProcessType.None; }
        }
        public override Dictionary<string,string> Parameters
        {
            get {return new Dictionary<string, string>();}
        }
    }
}