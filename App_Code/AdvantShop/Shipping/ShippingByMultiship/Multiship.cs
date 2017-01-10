//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Repository;
using Newtonsoft.Json;

namespace AdvantShop.Shipping
{
    public class Multiship : IShippingMethod
    {
        #region Fields

        private const string Url = "https://multiship.ru/OpenAPI_v3";

        private readonly string _cityFrom;
        private readonly string _clientId;
        private readonly string _secretKeyDelivery;

        public float WeightAvg;
        public int HeightAvg;
        public int WidthAvg;
        public int LengthAvg;

        
        public string CityTo { get; set; }
        public float TotalPrice { get; set; }
        public string WidgetCode { get; set; }

        private float Weight { get; set; }
        private int Height { get; set; }
        private int Width { get; set; }
        private int Length { get; set; }

        public int PickUpId { get; set; }

        public ShoppingCart ShoppingCart { get; set; }

        #endregion
        
        public Multiship(Dictionary<string, string> parameters)
        {
            _cityFrom = parameters.ElementOrDefault(MultishipTemplate.CityFrom);
            _clientId = parameters.ElementOrDefault(MultishipTemplate.ClientId);
            _secretKeyDelivery = parameters.ElementOrDefault(MultishipTemplate.SecretKeyDelivery);

            WeightAvg = parameters.ElementOrDefault(MultishipTemplate.WeightAvg).TryParseFloat();
            HeightAvg = Convert.ToInt32(parameters.ElementOrDefault(MultishipTemplate.HeightAvg));
            WidthAvg = Convert.ToInt32(parameters.ElementOrDefault(MultishipTemplate.WidthAvg));
            LengthAvg = Convert.ToInt32(parameters.ElementOrDefault(MultishipTemplate.LengthAvg));

            WidgetCode = parameters.ElementOrDefault(MultishipTemplate.WidgetCode);
        }

        public void Init()
        {
            var items = new List<string[]>();

            foreach (var item in ShoppingCart)
            {
                if (item.Offer.Product != null)
                {
                    var size = item.Offer.Product.Size.Split('|');
                    for (int i = 0; i < item.Amount; i++)
                    {
                        items.Add(size);
                    }
                }
            }

            var weight = ShoppingCart.TotalShippingWeight;
            var dimensions = MeasureHelper.GetDimensions(items, HeightAvg, WidthAvg, LengthAvg);
            
            Length = dimensions[0];
            Width = dimensions[1];
            Height = dimensions[2];
            Weight = weight > 0 ? weight : WeightAvg;
        }

        public List<ShippingOption> GetShippingOptions()
        {
            return GetOptions();
        }

        public List<ShippingOption> GetOptions(bool isCombine = true)
        {
            Init();

            var webRequest = (HttpWebRequest)WebRequest.Create(Url + "/searchDeliveryList");
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            webRequest.Accept = "text/json";

            var data = new Dictionary<string, object>()
            {
                {"client_id", _clientId},
                {"city_from", _cityFrom},
                {"city_to", CityTo},
                {"weight", Weight.ToString("F3").Replace(",", ".")},
                {"height", Height},
                {"width", Width},
                {"length", Length},
                {"total_cost", TotalPrice.ToString("F2").Replace(",", ".")}
            };


            var dataPost = string.Format("{0}&secret_key={1}",
                                    String.Join("&", data.Select(x => x.Key + "=" + x.Value)),
                                    MultishipService.GetSign(data, _secretKeyDelivery));

            var bytes = Encoding.UTF8.GetBytes(dataPost);
            webRequest.ContentLength = bytes.Length;
            using (var requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }

            try
            {
                using (var response = webRequest.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                var result = reader.ReadToEnd();
                                return ParseAnswer(result, isCombine);
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return new List<ShippingOption>();
        }

        private List<ShippingOption> ParseAnswer(string resultJson, bool isCombine)
        {
            var shippingOptions = new List<ShippingOption>();
            if (String.IsNullOrEmpty(resultJson))
                return shippingOptions;

            if (resultJson.Contains("error"))
            {
                return shippingOptions;
            }

            try
            {
                var data = JsonConvert.DeserializeObject<MultishipTemplate.MultishipAnswer>(resultJson);

                if (data.status != "ok")
                    return shippingOptions;

                shippingOptions.AddRange(
                    data.data.Where(item => item.pickuppoint_id.IsNullOrEmpty())
                             .Select(item => new ShippingOption
                             {
                                 Name = item.delivery_name,
                                 Rate = item.cost_with_rules,
                                 DeliveryTime = !item.days.Contains("дн") ? item.days + " дн" : ""
                             }));

                var pickupPoints = data.data.Where(item => item.pickuppoint_id.IsNotEmpty()).ToList();
                if (pickupPoints.Count > 0)
                {
                    if (isCombine)
                    {
                        var point = pickupPoints.FirstOrDefault(p => p.pickuppoint_id == PickUpId.ToString() && PickUpId != 0) ?? pickupPoints.OrderBy(p => p.cost_with_rules).First();

                        shippingOptions.Insert(0, new ShippingOption()
                        {
                            Name = "Постаматы и пункты самовывоза",
                            Rate = point.cost_with_rules,
                            IsMinimumRate = true,
                            DeliveryTime = !point.days.Contains("дн") ? point.days + " дн" : "",
                            Extend =
                                GetExtended(point.cost_with_rules, point.cost_with_rules, point.cost_with_rules,
                                    point.pickuppoint_id)
                        });
                    }
                    else
                    {
                        shippingOptions.AddRange(pickupPoints.Select(item => new ShippingOption()
                        {
                            Name = "Курьерская доставка " + item.delivery_name,
                            Rate = item.cost_with_rules,
                            DeliveryTime = string.Empty,
                            Extend =
                                GetExtended(item.cost_with_rules, item.cost_with_rules, item.cost_with_rules,
                                    item.pickuppoint_id)
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return shippingOptions;
        }

        private static ShippingOptionEx GetExtended(float bacePice, float pricecash, float transfer, string pickuppointId)
        {
            if (!string.IsNullOrEmpty(pickuppointId))
                return new ShippingOptionEx
                {
                    PickpointId = pickuppointId,
                    Type = ExtendedType.Pickpoint
                };

            return new ShippingOptionEx
            {
                BasePrice = bacePice,
                PriceCash = pricecash,
                Transfer = transfer,
                Type = ExtendedType.CashOnDelivery
            };
        }


        public float GetRate()
        {
            throw new Exception("GetRate method isnot avalible for MultiShip");
        }
    }
}