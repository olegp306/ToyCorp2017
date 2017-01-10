//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using AdvantShop.Diagnostics;
using AdvantShop.Repository;

namespace AdvantShop.Shipping
{
    public class ShippingByWeight : IShippingMethod
    {
        private readonly float _pricePerKg;
        private readonly float _extracharge;
        public ShippingByWeight(Dictionary<string, string> parameters)
        {
            try
            {
                _pricePerKg = parameters.ElementOrDefault(ShippingByWeightTemplate.PricePerKg).TryParseFloat();
                _extracharge = parameters.ElementOrDefault(ShippingByWeightTemplate.Extracharge).TryParseFloat();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                _pricePerKg = 0;
            }
        }

        public float GetRate(float weight, MeasureUnits.WeightUnit unit)
        {
            return GetRate(MeasureUnits.ConvertWeight(weight, unit, MeasureUnits.WeightUnit.Kilogramm));
        }

        public float GetRate(float weightInKg)
        {
            return (weightInKg * _pricePerKg) + _extracharge;
        }

        public float GetRate()
        {
            throw new Exception("GetRate method isnot avalible for ShippingByWeight");
        }

        public List<ShippingOption> GetShippingOptions()
        {
            throw new Exception("GetShippingOptions method isnot avalible for ShippingByWeight");
        }
    }
}