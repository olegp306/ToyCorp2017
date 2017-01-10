//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;

namespace AdvantShop.Payment
{
    /// <summary>
    /// Summary description for Cash
    /// </summary>
    public class Cash : PaymentMethod
    {
        public override PaymentType Type
        {
            get { return PaymentType.Cash; }
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