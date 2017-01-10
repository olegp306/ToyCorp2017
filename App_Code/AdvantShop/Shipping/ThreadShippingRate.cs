//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;

namespace AdvantShop.Shipping
{
    public class ThreadShippingRate
    {
        private readonly ShippingMethod _method;
        private int _index;
        private readonly string _countryName;
        private readonly string _countryIso2;
        private readonly string _currencyIso3;
        private readonly string _zipTo;
        private readonly string _cityTo;
        private readonly string _regionTo;
        private readonly ShoppingCart _shoppingCart;
        private readonly float _totalPrice;
        private readonly float _totalDiscount;
        private readonly int _distance;
        private readonly int _pickupId;

        private readonly List<OrderItem> _orderItems;

        private readonly List<ShippingItem> _shippingRates;
        public List<ShippingItem> ShippingRates
        {
            get { return _shippingRates; }
        }

        public ThreadShippingRate(ShippingMethod method, int index, string countryName, string countryIso2, string zipTo,
            string cityTo, string regionTo, string currencyIso3, ShoppingCart shoppingCart, float totalPrice,
            float totalDiscount, int distance, int pickupId = 0, List<OrderItem> orderItems = null)
        {
            _method = method;
            _index = index;
            _countryName = countryName;
            _countryIso2 = countryIso2;
            _currencyIso3 = currencyIso3;
            _zipTo = zipTo;
            _cityTo = cityTo;
            _regionTo = regionTo;
            _shoppingCart = shoppingCart;
            _totalPrice = totalPrice;
            _totalDiscount = totalDiscount;
            _distance = distance;
            _pickupId = pickupId;
            _shippingRates = new List<ShippingItem>();

            _orderItems = orderItems;
        }

        public void GetRate()
        {
            try
            {
                switch (_method.Type)
                {
                    case ShippingType.ShippingByWeight:
                        _shippingRates.AddRange(GetShippingByWeightRates());
                        break;
                    case ShippingType.ShippingByOrderPrice:
                        _shippingRates.AddRange(GetShippingByOrderPrice());
                        break;

                    case ShippingType.FedEx:
                        _shippingRates.AddRange(GetFedExRates());
                        break;

                    case ShippingType.Ups:
                        _shippingRates.AddRange(GetUpsRates());
                        break;

                    case ShippingType.Usps:
                        _shippingRates.AddRange(GetUspsRates());
                        break;

                    case ShippingType.FixedRate:
                        _shippingRates.AddRange(GetFixedRates());
                        break;

                    case ShippingType.FreeShipping:
                        _shippingRates.AddRange(GetFreeShippingRates());
                        break;
                    case ShippingType.eDost:
                        var edostRates = GetEdostShippingRates();
                        if (edostRates != null)
                            _shippingRates.AddRange(edostRates);
                        break;

                    case ShippingType.ShippingByShippingCost:
                        _shippingRates.AddRange(GetShippingByShippingCost());
                        break;

                    case ShippingType.ShippingByRangeWeightAndDistance:
                        _shippingRates.AddRange(GetShippingByRangeWeightAndDistance());
                        break;

                    case ShippingType.ShippingNovaPoshta:
                        _shippingRates.AddRange(GetNovaPoshtaShippingRates());
                        break;

                    case ShippingType.SelfDelivery:
                        _shippingRates.AddRange(GetSelfDeliveryShippingRates());
                        break;

                    case ShippingType.Multiship:
                        _shippingRates.AddRange(GetMultishipShippingRates());
                        break;

                    case ShippingType.Cdek:
                        _shippingRates.AddRange(GetCdekRates());
                        break;

                    case ShippingType.ShippingByProductAmount:
                        _shippingRates.AddRange(GetShippingByProductAmount());
                        break;

                    case ShippingType.ShippingByEmsPost:
                        _shippingRates.AddRange(GetShippingByEmsPost());
                        break;

                    case ShippingType.CheckoutRu:
                        _shippingRates.AddRange(GetCheckoutRuRates());
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private IEnumerable<ShippingItem> GetCdekRates()
        {
            return new Cdek(_method.Params)
            {
                //addressTo
                CountryTo = _countryName,
                CityTo = _cityTo,
                ZipTo = _zipTo,
                StateTo = _regionTo,
                //addressFrom
                CountryFrom = _countryName,
                ZipFrom = _zipTo,
                StateFrom = _regionTo,

                MethodName = _method.Name,
                ShoppingCart = _shoppingCart,
                TotalPrice = _totalPrice - _totalDiscount
            }
                .GetShippingOptions()
                .Select(
                    item =>
                        new ShippingItem(_method)
                        {
                            Id = ++_index,
                            Type = ShippingType.Cdek,
                            MethodNameRate = item.Name,
                            Rate = item.Rate,
                            ShippingPoints = item.ShippingPoints,
                            Ext = GetItemExt(item.Extend),
                            DeliveryTime = item.DeliveryTime

                        });
        }
        
        private IEnumerable<ShippingItem> GetShippingByRangeWeightAndDistance()
        {
            return new List<ShippingItem>
            {
                new ShippingItem(_method)
                {
                    Id = ++_index,
                    Type = ShippingType.ShippingByRangeWeightAndDistance,
                    Params = _method.Params,
                    DeliveryTime = _method.Params.ElementOrDefault(ShippingByOrderPriceTemplate.DeliveryTime),
                    Rate =
                        new ShippingByRangeWeightAndDistance(_method.Params) {ShoppingCart = _shoppingCart}.GetRate(_distance)
                }
            };
        }

        private IEnumerable<ShippingItem> GetShippingByWeightRates()
        {
            return new List<ShippingItem>
            {
                new ShippingItem(_method)
                {
                    Id = ++_index,
                    Type = ShippingType.ShippingByWeight,
                    Rate = new ShippingByWeight(_method.Params).GetRate(_shoppingCart.TotalShippingWeight),
                    DeliveryTime = _method.Params.ElementOrDefault(ShippingByWeightTemplate.DeliveryTime)
                }
            };
        }

        private IEnumerable<ShippingItem> GetShippingByShippingCost()
        {
            return new List<ShippingItem>
            {
                new ShippingItem(_method)
                {
                    Id = ++_index,
                    Type = ShippingType.ShippingByShippingCost,
                    IconName = _method.IconFileName.PhotoName,
                    Rate = new ShippingByShippingCost(_method.Params) {ShoppingCart = _shoppingCart}.GetRate()
                }
            };
        }

        private IEnumerable<ShippingItem> GetShippingByProductAmount()
        {
            var list = new List<ShippingItem>();
            var item = new ShippingItem(_method)
            {
                Id = ++_index,
                Type = ShippingType.ShippingByProductAmount,
                IconName = _method.IconFileName.PhotoName,
                Rate = new ShippingByProductAmount(_method.Params).GetRate(_shoppingCart.TotalItems)
            };

            if (item.Rate != -1)
                list.Add(item);

            return list;
        }

        private IEnumerable<ShippingItem> GetShippingByEmsPost()
        {
            return new ShippingByEmsPost(_method.Params)
                {
                    CityTo = _cityTo,
                    RegionTo = _regionTo,
                    CountryTo = _countryName,
                    ShoppingCart = _shoppingCart,
                }
                .GetShippingOptions()
                .Select(
                    item => new ShippingItem(_method)
                    {
                        Id = ++_index,
                        Type = ShippingType.ShippingByEmsPost,
                        DeliveryTime = item.DeliveryTime,
                        Rate = item.Rate,
                    });
        }

        private IEnumerable<ShippingItem> GetShippingByOrderPrice()
        {
            var shippingitem = new ShippingItem(_method)
            {
                Id = ++_index,
                Type = ShippingType.ShippingByOrderPrice,
                DeliveryTime = _method.Params.ElementOrDefault(ShippingByOrderPriceTemplate.DeliveryTime),
                Rate = new ShippingByOrderPrice(_method.Params).GetRate(_totalPrice, _totalPrice - _totalDiscount),
            };

            var list = new List<ShippingItem>();

            if (shippingitem.Rate != -1)
                list.Add(shippingitem);

            return list;
        }
        
        private IEnumerable<ShippingItem> GetFedExRates()
        {
            return new FedEx(_method.Params)
            {
                CountryCodeTo = _countryIso2,
                PostalCodeTo = _zipTo,
                ShoppingCart = _shoppingCart,
                CurrencyIso3 = _currencyIso3,
                TotalPrice = _totalPrice - _totalDiscount
            }
                .GetShippingOptions()
                .Select(
                    item => new ShippingItem(_method)
                    {
                        Id = ++_index,
                        Type = ShippingType.FedEx,
                        MethodNameRate = item.Name,
                        DeliveryTime = item.DeliveryTime,
                        Rate = item.Rate,
                    });
        }

        private IEnumerable<ShippingItem> GetUpsRates()
        {
            return new Ups(_method.Params)
            {
                CountryCodeTo = _countryIso2,
                PostalCodeTo = _zipTo,
                ShoppingCart = _shoppingCart
            }
                .GetShippingOptions()
                .Select(
                    item => new ShippingItem(_method)
                    {
                        Id = ++_index,
                        Type = ShippingType.Ups,
                        MethodNameRate = item.Name,
                        Rate = item.Rate,
                    });

        }

        private IEnumerable<ShippingItem> GetUspsRates()
        {
            return new Usps(_method.Params)
            {
                CountryTo = _countryName,
                CountryToIso2 = _countryIso2,
                PostalCodeTo = _zipTo,
                Size = Usps.PackageSize.Regular,
                ShoppingCart = _shoppingCart,
                TotalPrice = _totalPrice - _totalDiscount
            }
                .GetShippingOptions()
                .Select(
                    item =>
                        new ShippingItem(_method)
                        {
                            Id = ++_index,
                            Type = ShippingType.Usps,
                            MethodNameRate = item.Name,
                            Rate = item.Rate,
                        });
        }
        
        private IEnumerable<ShippingItem> GetFixedRates()
        {
            return new List<ShippingItem>
            {
                new ShippingItem(_method)
                {
                    Id = ++_index,
                    Type = ShippingType.FixedRate,
                    Rate = new FixeRateShipping(_method.Params).GetRate(),
                    DeliveryTime = _method.Params.ElementOrDefault(FixeRateShippingTemplate.DeliveryTime),
                }
            };
        }

        private IEnumerable<ShippingItem> GetFreeShippingRates()
        {
            return new List<ShippingItem>
            {
                new ShippingItem(_method)
                {
                    Id = ++_index,
                    Type = ShippingType.FreeShipping,
                    Rate = new FreeShipping().GetRate(),
                    DeliveryTime = _method.Params.ElementOrDefault(FreeShippingTemplate.DeliveryTime),
                }
            };
        }

        private IEnumerable<ShippingItem> GetSelfDeliveryShippingRates()
        {
            return new List<ShippingItem>
            {
                new ShippingItem(_method)
                {
                    Id = ++_index,
                    Type = ShippingType.SelfDelivery,
                    Rate = new SelfDelivery(_method.Params).GetRate(),
                    DeliveryTime = _method.Params.ElementOrDefault(SelfDeliveryTemplate.DeliveryTime)
                }
            };
        }

        private IEnumerable<ShippingItem> GetNovaPoshtaShippingRates()
        {
            var novaPoshta = new NovaPoshta(_method.Params)
                {
                    CityTo = _cityTo,
                    TotalWeight = _shoppingCart.TotalShippingWeight,
                    TotalPrice = _totalPrice - _totalDiscount,
                };

            var novaPoshtaShipping = novaPoshta.GetShippingOption();
            if (novaPoshtaShipping == null)
                return new List<ShippingItem>();

            return new List<ShippingItem>
            {
                new ShippingItem(_method)
                {
                    Id = ++_index,
                    Type = ShippingType.ShippingNovaPoshta,
                    Rate = novaPoshtaShipping.Rate,
                    DeliveryTime = novaPoshtaShipping.DeliveryTime,
                }
            };
        }

        private IEnumerable<ShippingItem> GetEdostShippingRates()
        {
            var edost = new Edost(_method.Params)
                {
                    ShippingId = _method.ShippingMethodId,
                    CityTo = _cityTo,
                    Zip = _zipTo,
                    ShoppingCart = _shoppingCart,
                    TotalPrice = _totalPrice - _totalDiscount,
                    PickPointID = _pickupId
                };

            var edostShippings = edost.GetShippingOptions();
            if (edostShippings == null)
                return null;


            if (!edostShippings.Any())
            {
                edost.CityTo = _cityTo + " (" + _regionTo + ")";
                edostShippings = edost.GetShippingOptions();
            }

            if (!edostShippings.Any())
            {
                edost.CityTo = _regionTo;
                edostShippings = edost.GetShippingOptions();
            }
            if (edostShippings == null)
                return null;

            if (!edostShippings.Any())
            {
                edost.CityTo = _countryName;
                edostShippings = edost.GetShippingOptions();
            }

            if (edostShippings == null)
                return null;

            return edostShippings.Select(
                item =>
                    new ShippingItem(_method)
                    {
                        Id = ++_index,
                        Type = ShippingType.eDost,
                        MethodNameRate = item.Name,
                        Rate = item.Rate,
                        DeliveryTime = item.DeliveryTime,
                        Ext = GetItemExt(item.Extend),
                        ShippingPoints = item.ShippingPoints
                    });
        }

        private IEnumerable<ShippingItem> GetMultishipShippingRates()
        {
            var multiship = new Multiship(_method.Params)
            {
                CityTo = _cityTo,
                ShoppingCart = _shoppingCart,
                TotalPrice = _totalPrice - _totalDiscount,
                PickUpId = _pickupId
            };

            var multishipOptions = multiship.GetShippingOptions();
            if (multishipOptions == null || !multishipOptions.Any())
                return new List<ShippingItem>();

            return multishipOptions.Select(item => new ShippingItem(_method)
            {
                Id = ++_index,
                Type = ShippingType.Multiship,
                MethodNameRate = item.Name,
                Rate = item.Rate,
                IsMinimumRate = item.IsMinimumRate,
                DeliveryTime = item.DeliveryTime,
                Ext = GetItemExt(item.Extend)
            });
        }


        private IEnumerable<ShippingItem> GetCheckoutRuRates()
        {
            return new ShippingCheckoutRu(_method.Params)
            {
                CountryTo = _countryName,
                CityTo = _cityTo,
                ZipTo = _zipTo,
                StateTo = _regionTo,
                
                ShoppingCart = _shoppingCart,
                OrderItems = _orderItems
            }
                .GetShippingOptions()
                .Select(
                    item =>
                        new ShippingItem(_method)
                        {
                            Id = ++_index,
                            Type = ShippingType.CheckoutRu,
                            MethodNameRate = item.Name,
                            Rate = item.Rate,
                            ShippingPoints = item.ShippingPoints,
                            Ext = GetItemExt(item.Extend)
                        });
        }

        private ShippingOptionEx GetItemExt(ShippingOptionEx item)
        {
            if (item != null)
            {
                item.ShippingId = _method.ShippingMethodId;
            }
            return item;
        }
    }
}
