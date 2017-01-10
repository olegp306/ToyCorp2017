using System;
using System.Collections.Generic;
using AdvantShop.Orders;

namespace AdvantShop.Shipping
{
    public class ShippingByEmsPost : IShippingMethod
    {
        #region Fields

        private readonly float _defaultWeight;
        private readonly float _maxWeight;
        private readonly float _extraPrice;
        private readonly string _cityFrom;

        public string CityTo { get; set; }
        public string RegionTo { get; set; }
        public string CountryTo { get; set; }
        public ShoppingCart ShoppingCart { get; set; }

        #endregion

        public ShippingByEmsPost(Dictionary<string, string> parameters)
        {
            _defaultWeight = Convert.ToSingle(parameters.ElementOrDefault(ShippingByEmsPostTemplate.DefaultWeight));
            _maxWeight = Convert.ToSingle(parameters.ElementOrDefault(ShippingByEmsPostTemplate.MaxWeight));
            _extraPrice = Convert.ToSingle(parameters.ElementOrDefault(ShippingByEmsPostTemplate.ExtraPrice));
            _cityFrom = parameters.ElementOrDefault(ShippingByEmsPostTemplate.CityFrom);
        }

        public List<ShippingOption> GetShippingOptions()
        {
            var options = new List<ShippingOption>();

            var weight = ShoppingCart.TotalShippingWeight;
            if (weight == 0)
                weight = _defaultWeight;

            if (_maxWeight != 0 && weight >= _maxWeight)
                return options;

            var emsPrice = ShippingEmsPostService.GetEmsPriceByCity(_cityFrom, CityTo, weight);
            if (emsPrice == null && RegionTo.IsNotEmpty())
            {
                emsPrice = ShippingEmsPostService.GetEmsPriceByRegion(_cityFrom, RegionTo, weight);
                if (emsPrice == null && CountryTo.IsNotEmpty())
                {
                    emsPrice = ShippingEmsPostService.GetEmsPriceByCountry(_cityFrom, CountryTo, weight);
                }
            }

            if (emsPrice == null)
                return options;

            options.Add(new ShippingOption()
            {
                Rate = emsPrice.price + _extraPrice,
                DeliveryTime =
                    emsPrice.term != null
                        ? (emsPrice.term["min"] == emsPrice.term["max"]
                            ? string.Format("{0} дн.", emsPrice.term["min"])
                            : string.Format("{0} - {1} дн.", emsPrice.term["min"], emsPrice.term["max"]))
                        : string.Empty
            });

            return options;
        }

        public float GetRate()
        {
            throw new Exception("GetShippingOptions method isnot avalible for ShippingByEmsPost");
        }
    }
}