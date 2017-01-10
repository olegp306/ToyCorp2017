//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;

namespace AdvantShop.Payment
{
    public class PickPoint : PaymentMethod
    {
        public int ShippingMethodId { get; set; }

        public override PaymentType Type
        {
            get { return PaymentType.PickPoint; }
        }

        public override ProcessType ProcessType
        {
            get { return ProcessType.None; }
        }

        public const string ShippingMethodTemplate = "ShippingMethod";

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                             {
                                 {ShippingMethodTemplate,ShippingMethodId.ToString( )}
                             };
            }
            set
            {
                int decVal;
                ShippingMethodId = value.ContainsKey(ShippingMethodTemplate) && int.TryParse(value[ShippingMethodTemplate], out decVal) ? decVal : 0;
            }
        }

    }
}