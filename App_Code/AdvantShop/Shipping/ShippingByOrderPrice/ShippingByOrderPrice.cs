using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Diagnostics;

namespace AdvantShop.Shipping
{
    [Serializable]
    public class ShippingPriceRange
    {
        public float OrderPrice { get; set;}
        public float ShippingPrice { get; set; }

        public override string ToString()
        {
            return OrderPrice + "=" + ShippingPrice;
        }
    }

    public class ShippingByOrderPrice : IShippingMethod
    {
        private readonly List<ShippingPriceRange> _priceRanges;
        private readonly bool _dependsOnCartPrice;

        public ShippingByOrderPrice(Dictionary<string, string> parameters)
        {
            try
            {
                _priceRanges = new List<ShippingPriceRange>();

                var ranges = parameters.ElementOrDefault(ShippingByOrderPriceTemplate.PriceRanges);
                if (ranges.IsNullOrEmpty())
                    return;

                foreach (var item in ranges.Split(';'))
                {
                    if (item.Split('=').Length == 2)
                    {
                        _priceRanges.Add(new ShippingPriceRange()
                            {
                                OrderPrice = item.Split('=')[0].TryParseFloat(),
                                ShippingPrice = item.Split('=')[1].TryParseFloat()
                            });
                    }
                }

                _priceRanges = _priceRanges.OrderBy(item => item.OrderPrice).ToList();
                _dependsOnCartPrice = parameters.ElementOrDefault(ShippingByOrderPriceTemplate.DependsOnCartPrice).TryParseBool();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                _priceRanges = new List<ShippingPriceRange>();
            }
        }

        public float GetRate(float cartPrice, float orderPrice)
        {
            float comparePrice = _dependsOnCartPrice ? cartPrice : orderPrice;
            float shippingPrice = -1;
            foreach(var range in _priceRanges)
            {
                if (comparePrice >= range.OrderPrice)
                {
                    shippingPrice = range.ShippingPrice;
                }
            }

            return shippingPrice;
        }

        public float GetRate()
        {
            throw new Exception("GetRate method isnot avalible for ShippingByOrderPrice");
        }

        public List<ShippingOption> GetShippingOptions()
        {
            throw new Exception("GetShippingOptions method isnot avalible for ShippingByOrderPrice");
        }
    }
}