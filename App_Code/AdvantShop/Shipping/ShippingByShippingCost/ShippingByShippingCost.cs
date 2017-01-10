//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using System.Linq;

namespace AdvantShop.Shipping
{
    public class ShippingByShippingCost : IShippingMethod
    {
        public ShoppingCart ShoppingCart { get; set; }
        private readonly bool _byMaxShippingCost;
        private readonly bool _useAmount;

        public ShippingByShippingCost(Dictionary<string, string> parameters)
        {
            _byMaxShippingCost = parameters.ElementOrDefault(ShippingByShippingCostTemplate.ByMaxShippingCost).TryParseBool();
            _useAmount = parameters.ElementOrDefault(ShippingByShippingCostTemplate.UseAmount).TryParseBool();
        }

        public float GetRate()
        {
            if (ShoppingCart.Any())
            {
                if (!_useAmount)
                {
                    return _byMaxShippingCost
                               ? ShoppingCart.Max(item => item.Offer.Product.ShippingPrice)
                               : ShoppingCart.Sum(item => item.Offer.Product.ShippingPrice);
                }
                else
                {
                    return _byMaxShippingCost
                               ? ShoppingCart
                                     .Max(item => item.Offer.Product.ShippingPrice * item.Amount)

                               : ShoppingCart
                                     .Sum(item => item.Offer.Product.ShippingPrice * item.Amount);
                }
            }
            else
                return 0F;
        }

        public List<ShippingOption> GetShippingOptions()
        {
            throw new Exception("GetShippingOptions method isnot avalible for ShippingByShippingCost");
        }
    }
}