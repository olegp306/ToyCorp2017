//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;

namespace AdvantShop.Shipping
{
    public class SelfDelivery : IShippingMethod
    {
        private readonly float _shippingPrice;

        public SelfDelivery(Dictionary<string, string> parameters)
        {
            _shippingPrice = parameters.ElementOrDefault(FixeRateShippingTemplate.ShippingPrice) != null
                                 ? parameters.ElementOrDefault(FixeRateShippingTemplate.ShippingPrice).TryParseFloat()
                                 : 0;
        }

        public float GetRate()
        {
            return _shippingPrice;
        }

        public List<ShippingOption> GetShippingOptions()
        {
            throw new Exception("GetShippingOptions method isnot avalible for SelfDelivery");
        }
    }
}