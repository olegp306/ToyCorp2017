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
using System.Xml;
using System.Xml.Linq;
using AdvantShop.Catalog;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Repository;

namespace AdvantShop.Shipping
{
    public class Edost : IShippingMethod
    {
        private const string Url = "http://www.edost.ru/edost_calc_kln.php";
        public string ShopId { get; set; }
        public string Password { get; set; }
        public bool Insurance { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public float TotalPrice { get; set; }
        public string CityTo { get; set; }
        public string Zip { get; set; }
        public float Rate { get; set; }
        public int PickPointID { get; set; }
        public int ShippingId { get; set; }

        public Edost(Dictionary<string, string> parameters)
        {
            ShopId = parameters.ElementOrDefault(EdostTemplate.ShopId);
            Password = parameters.ElementOrDefault(EdostTemplate.Password);
            Rate = parameters.ElementOrDefault(EdostTemplate.Rate).TryParseFloat();
        }

        /// <summary>
        /// Don't use this for Edost
        /// </summary>
        /// <returns></returns>
        public float GetRate()
        {
            throw new Exception("GetRate method isnot avalible for Edost");
        }

        public List<ShippingOption> GetShippingOptions()
        {
            string postData = GetParam();
            var hash = postData.GetHashCode();
            string serverResponse;

            ShippingCacheRepositiry.Delete();
            var cached = ShippingCacheRepositiry.Get(ShippingId, hash);
            if (cached != null)
                serverResponse = cached.ServerResponse;
            else
            {
                try
                {
                    ServicePointManager.Expect100Continue = false;
                    var request = WebRequest.Create(Url);
                    request.Method = "POST";

                    byte[] byteArray = Encoding.GetEncoding("windows-1251").GetBytes(postData);

                    // Set the ContentType property of the WebRequest.
                    request.ContentType = "application/x-www-form-urlencoded";
                    // Set the ContentLength property of the WebRequest.
                    request.ContentLength = byteArray.Length;
                    // Get the request stream.
                    using (Stream dataStream = request.GetRequestStream())
                    {
                        // Write the data to the request stream.
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        // Close the Stream object.
                        dataStream.Close();
                    }
                    using (var response = request.GetResponse())
                    {
                        // Get the stream containing all content returned by the requested server.
                        using (var dataStream = response.GetResponseStream())
                        {
                            if (dataStream == null) return new List<ShippingOption>();
                            // Open the stream using a StreamReader for easy access.
                            using (var reader = new StreamReader(dataStream))
                            {
                                // Read the content fully up to the end.
                                serverResponse = reader.ReadToEnd();

                                var model = new ShippingCache
                                {
                                    ServerResponse = serverResponse,
                                    ShippingMethodId = ShippingId,
                                    ParamHash = hash
                                };
                                ShippingCacheRepositiry.Add(model);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                    return null;
                }
            }
            return ParseAnswer(serverResponse);
        }

        private string GetParam()
        {


            var items = new List<string[]>();

            foreach (var item in ShoppingCart)
            {
                var size = item.Offer.Product.Size.Split('|');
                for (int i = 0; i < item.Amount; i++)
                {
                    items.Add(size);
                }
            }

            var heightAvg = 10;
            var widthAvg = 10;
            var lengthAvg = 10;

            var dimensions = MeasureHelper.GetDimensions(items, heightAvg, widthAvg, lengthAvg);

            var length = dimensions[0];
            var width = dimensions[1];
            var height = dimensions[2];

            var a = new StringBuilder();
            a.Append("to_city=" + CityTo);
            a.Append("&zip=" + Zip);
            a.Append("&weight=" + ShoppingCart.TotalShippingWeight.ToString("F3"));
            a.Append("&strah=" + (Rate != 0 ? TotalPrice / Rate : TotalPrice).ToString("F2"));
            a.Append("&id=" + ShopId);
            a.Append("&p=" + Password);
            a.Append("&ln=" + length);
            a.Append("&wd=" + width);
            a.Append("&hg=" + height);
            return a.ToString();
        }

        private List<ShippingOption> ParseAnswer(string responseFromServer)
        {
            if (responseFromServer.IsNullOrEmpty())
                return new List<ShippingOption>();

            var shippingOptions = new List<ShippingOption>();

            var doc = XDocument.Parse(responseFromServer);

            if (doc.Root == null)
                return shippingOptions;

            var status = doc.Root.Element("stat");
            if (status != null && status.Value != "1")
            {
                GetErrorEdost(status.Value, CityTo);
                return shippingOptions;
            }

            foreach (var el in doc.Root.Elements("tarif"))
            {
                var idEl = el.Element("id");
                var priceEl = el.Element("price");
                var priceCashEl = el.Element("pricecash");
                var priceTransferEl = el.Element("transfer");
                var nameEl = el.Element("name");
                var pickpointMapEl = el.Element("pickpointmap");
                var companyEl = el.Element("company");
                var dayEl = el.Element("day");

                if (idEl == null || priceEl == null || nameEl == null || companyEl == null)
                    continue;

                var price = priceEl.Value.TryParseFloat() * Rate;
                var priceCash = priceCashEl != null ? priceCashEl.Value.TryParseFloat() * Rate : 0;
                var priceTransfer = priceTransferEl != null ? priceTransferEl.Value.TryParseFloat() * Rate : 0;

                var pickpointmap = pickpointMapEl != null ? pickpointMapEl.Value : string.Empty;

                var shippingOption = new ShippingOption
                {
                    ShippingMethodId = Convert.ToInt32(idEl.Value),
                    Name = companyEl.Value + (string.IsNullOrWhiteSpace(nameEl.Value) ? string.Empty : " (" + nameEl.Value + ")"),
                    Rate = price,
                    DeliveryTime = dayEl.Value,
                    // extension наложеный платеж или пикпойнт
                    Extend =
                        priceCash != 0 || priceTransfer != 0 || pickpointmap.IsNotEmpty()
                            ? GetExtended(price, priceCash, priceTransfer, pickpointmap)
                            : null
                };

                if (pickpointmap.IsNotEmpty())
                    shippingOptions.Insert(0, shippingOption);
                else
                    shippingOptions.Add(shippingOption);
            }

            bool first = true;
            foreach (var el in doc.Root.Elements("office"))
            {
                var name = el.Element("name");
                var addressEl = el.Element("address");
                var telEl = el.Element("tel");
                var schelduleEl = el.Element("schedule");
                var tarifId = el.Element("to_tarif").Value.TryParseFloat();

                var point = new ShippingPoint()
                {
                    Id = Convert.ToInt32(el.Element("id").Value),
                    Address = name.Value,
                    Description = string.Format("<div>{0}</div> <div>{1}</div> <div>{2}</div>",
                                            addressEl.Value, telEl.Value, schelduleEl.Value)
                };

                var option = shippingOptions.Find(o => o.ShippingMethodId == tarifId);
                if (option != null)
                {
                    option.ShippingPoints.Add(point);

                    if (first || point.Id == PickPointID)
                    {
                        if (option.Extend == null)
                        {
                            option.Extend = new ShippingOptionEx();
                        }

                        option.Extend.PickpointId = point.Id.ToString();
                        option.Extend.PickpointAddress = point.Address;
                        option.Extend.Type |= option.Name.ToLower().Contains("boxberry")
                            ? ExtendedType.Boxberry
                            : option.Name.ToLower().Contains("сдэк") ? ExtendedType.Cdek : ExtendedType.CashOnDelivery;
                        option.Extend.AdditionalData = point.Description;
                    }
                }
                first = false;
            }



            return shippingOptions;
        }

        public static ShippingOptionEx GetExtended(float bacePice, float pricecash, float transfer, string pickpointmap)
        {
            if (!string.IsNullOrEmpty(pickpointmap))
                return new ShippingOptionEx
                           {
                               Pickpointmap = pickpointmap,
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

        //1 - успех
        //2 - доступ к расчету заблокирован
        //3 - неверные данные магазина (пароль или идентификатор)
        //4 - неверные входные параметры
        //5 - неверный город или страна
        //6 - внутренняя ошибка сервера расчетов
        //7 - не заданы компании доставки в настройках магазина
        //8 - сервер расчета не отвечает
        //9 - превышен лимит расчетов за день
        //11 - не указан вес
        //12 - не заданы данные магазина (пароль или идентификатор)
        private static void GetErrorEdost(string str, string city)
        {
            try
            {
                switch (str)
                {
                    case "2":
                        throw new Exception("доступ к расчету заблокирован");

                    case "3":
                        throw new Exception("неверные данные магазина (пароль или идентификатор)");

                    case "4":
                        throw new Exception("неверные входные параметры");

                    case "5":
                        throw new Exception("неверный город или страна" + " город:" + city + "!");

                    case "6":
                        throw new Exception("внутренняя ошибка сервера расчетов");

                    case "7":
                        throw new Exception("не заданы компании доставки в настройках магазина");

                    case "8":
                        throw new Exception("сервер расчета не отвечает");

                    case "9":
                        throw new Exception("превышен лимит расчетов за день");

                    case "11":
                        throw new Exception("не указан вес");

                    case "12":
                        throw new Exception("не заданы данные магазина (пароль или идентификатор)");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex, false);
            }
        }
    }
}