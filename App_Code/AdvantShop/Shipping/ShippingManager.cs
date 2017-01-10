//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Shipping
{
    public class ShippingItem
    {
        public int Id { get; set; }
        public ShippingType Type { get; set; }
        public int MethodId { get; set; }
        public string MethodName { get; set; }
        public string MethodNameRate { get; set; }
        public string MethodDescription { get; set; }
        public float Rate { get; set; }
        public bool IsMinimumRate { get; set; }
        public string DeliveryTime { get; set; }
        public bool ShowInDetails { get; set; }
        public string ZeroPriceMessage { get; set; }
        
        public ShippingOptionEx Ext { get; set; }
        public List<ShippingPoint> ShippingPoints { get; set; }

        public Dictionary<string, string> Params { get; set; }

        public bool DisplayCustomFields { get; set; }
        public string IconName { get; set; }

        public ShippingItem()
        {
            ShippingPoints = new List<ShippingPoint>();
        }

        public ShippingItem(ShippingMethod method)
        {
            MethodId = method.ShippingMethodId;
            MethodName = method.Name;
            MethodNameRate = method.Name;
            MethodDescription = method.Description;
            DisplayCustomFields = method.DisplayCustomFields;
            IconName = method.IconFileName.PhotoName;
            ShowInDetails = method.ShowInDetails;
            ZeroPriceMessage = method.ZeroPriceMessage;
        }
    }

    public class ShippingManager
    {
        private readonly List<ShippingMethod> _listMethod;
        private static string _countryName = string.Empty;
        private static string _countryIso2 = string.Empty;
        private static string _zipTo = string.Empty;
        private static string _cityTo = string.Empty;
        private static string _regionTo = string.Empty;
        private static int _pickupId = 0;
        private static int _distance;
        private ShoppingCart _shoppingCart;

        private List<OrderItem> _orderItems;

        public int SelectIndex { get; set; }

        private Currency _currency = CurrencyService.CurrentCurrency;
        public Currency Currency
        {
            get { return _currency; }
            set { if (value != null) _currency = value; }
        }

        public ShippingManager()
        {
            _listMethod = ShippingMethodService.GetAllShippingMethods(true);
        }

        public List<ShippingItem> GetShippingRates()
        {
            return GetShippingRates(ShoppingCartService.CurrentShoppingCart);
        }

        public List<ShippingItem> GetShippingRates(ShoppingCart shoppingCart)
        {
            _countryName = string.Empty;
            _countryIso2 = string.Empty;
            _zipTo = string.Empty;
            _cityTo = string.Empty;
            _shoppingCart = shoppingCart;
            return GetShippingRatesList();
        }

        public List<ShippingItem> GetShippingRates(int countryId, string zip, string city, string region,
                                                    ShoppingCart shoppingCart, int distance = 0, int pickupId = 0, List<OrderItem> orderItems = null)
        {
            var country = CountryService.GetCountry(countryId);

            _countryName = country.Name;
            _countryIso2 = country.Iso2;
            _zipTo = zip;
            _cityTo = city;
            _shoppingCart = shoppingCart;
            _regionTo = region;
            _distance = distance;
            _pickupId = pickupId;

            _orderItems = orderItems;

            return GetShippingRatesList();
        }


        private List<ShippingItem> GetShippingRatesList()
        {
            if(_shoppingCart == null)
            {
                throw new Exception("_shoppingCart == null");
            }
            var lists = new List<ShippingItem>();

            var currencyIso3 = Currency.Iso3;

            var threads = new List<Thread>();
            var shippingRates = new List<ThreadShippingRate>();
            var totalPrice = _shoppingCart.TotalPrice;
            var totalDiscount = _shoppingCart.TotalDiscount;

            int i = 1;
            foreach (var item in GetShippingMethodsByGeoMapping(_listMethod))
            {
                int index = 1000*(int) item.Type + 100*i++;

                var shippingRate = new ThreadShippingRate(item, index, _countryName, _countryIso2, _zipTo, _cityTo,
                                                          _regionTo, currencyIso3, _shoppingCart, totalPrice, totalDiscount,
                                                          _distance, _pickupId, _orderItems);
                shippingRates.Add(shippingRate);

                var thread = new Thread(shippingRate.GetRate);
                thread.Start();
                threads.Add(thread);
            }

            foreach (var t in threads)
                t.Join();

            foreach (var shippingRate in shippingRates)
            {
                lists.AddRange(shippingRate.ShippingRates);
            }

            return lists;
        }

        private IEnumerable<ShippingMethod> GetShippingMethodsByGeoMapping(IEnumerable<ShippingMethod> listMethods)
        {
            var items = new List<ShippingMethod>();
            foreach (var shippingMethod in listMethods)
            {
                if (ShippingPaymentGeoMaping.IsExistGeoShipping(shippingMethod.ShippingMethodId))
                {
                    if (ShippingPaymentGeoMaping.CheckShippingEnabledGeo(shippingMethod.ShippingMethodId, _countryName, _cityTo))
                        items.Add(shippingMethod);
                }
                else
                    items.Add(shippingMethod);
            }
            return items;
        }

        public static List<ShippingItem> CurrentShippingRates
        {
            get
            {
                return HttpContext.Current.Session["ShippingRates"] != null
                           ? (List<ShippingItem>)HttpContext.Current.Session["ShippingRates"]
                           : new List<ShippingItem>();
            }
            set
            {
                HttpContext.Current.Session["ShippingRates"] = value;
            }
        }
    }
}