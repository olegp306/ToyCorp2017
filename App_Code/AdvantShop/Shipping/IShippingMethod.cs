//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;

namespace AdvantShop.Shipping
{
    public interface IShippingMethod
    {
        float GetRate();
        List<ShippingOption> GetShippingOptions();
    }
}