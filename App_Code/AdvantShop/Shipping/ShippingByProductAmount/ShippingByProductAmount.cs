using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Diagnostics;

namespace AdvantShop.Shipping
{
    [Serializable]
    public class ShippingAmountRange
    {
        public float Amount { get; set; }
        public float ShippingPrice { get; set; }

        public override string ToString()
        {
            return Amount + "=" + ShippingPrice;
        }
    }

    public class ShippingByProductAmount : IShippingMethod
    {
        private readonly List<ShippingAmountRange> _priceRanges;

        public ShippingByProductAmount(Dictionary<string, string> parameters)
        {
            try
            {
                _priceRanges = new List<ShippingAmountRange>();

                var ranges = parameters.ElementOrDefault(ShippingByProductAmountTemplate.PriceRanges);
                if (ranges.IsNullOrEmpty())
                    return;

                foreach (var item in ranges.Split(';'))
                {
                    if (item.Split('=').Length == 2)
                    {
                        _priceRanges.Add(new ShippingAmountRange()
                        {
                            Amount = item.Split('=')[0].TryParseFloat(),
                            ShippingPrice = item.Split('=')[1].TryParseFloat()
                        });
                    }
                }

                _priceRanges = _priceRanges.OrderBy(item => item.Amount).ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                _priceRanges = new List<ShippingAmountRange>();
            }
        }

        public float GetRate(float amount)
        {
            float shippingPrice = -1;
            foreach(var range in _priceRanges)
            {
                if (amount >= range.Amount)
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