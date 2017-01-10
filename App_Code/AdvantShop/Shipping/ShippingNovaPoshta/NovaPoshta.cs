using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;

namespace AdvantShop.Shipping
{
    public class NovaPoshta : IShippingMethod
    {
        private const string Url = "https://api.novaposhta.ua/v2.0/json/";
        private string ApiKey { get; set; }
        private enNovaPoshtaDeliveryType DeliveryType { get; set; }
        //private bool EnabledInsurance { get; set; }
        //private bool EnabledCod { get; set; }
        public float TotalPrice { get; set; }
        public float TotalWeight { get; set; }
        private string CityFrom { get; set; }
        public string CityTo { get; set; }
        private float Rate { get; set; }
        private const string Language = "ru";

        public NovaPoshta(Dictionary<string, string> parameters)
        {
            ApiKey = parameters.ElementOrDefault(NovaPoshtaTemplate.APIKey);
            CityFrom = parameters.ElementOrDefault(NovaPoshtaTemplate.CityFrom);
            DeliveryType = (enNovaPoshtaDeliveryType)parameters.ElementOrDefault(NovaPoshtaTemplate.DeliveryType).TryParseInt((int)enNovaPoshtaDeliveryType.WarehouseDoors);
            //return;
            //EnabledInsurance = parameters.ElementOrDefault(NovaPoshtaTemplate.EnabledInsurance).TryParseBool();
            //EnabledCod = parameters.ElementOrDefault(NovaPoshtaTemplate.EnabledCOD).TryParseBool();
            Rate = parameters.ElementOrDefault(NovaPoshtaTemplate.Rate).TryParseFloat();
        }

        public float GetRate()
        {
            throw new NotImplementedException();
        }

        public List<ShippingOption> GetShippingOptions()
        {
            throw new NotImplementedException();
        }

        public ShippingOption GetShippingOption()
        {
            try
            {
                var senderCity = GetCity(CityFrom, "");
                var recipientCity = GetCity(CityTo, "");
                var delivery = GetDocumentDeliveryDate(senderCity.Ref, recipientCity.Ref, enNovaPoshtaDeliveryType.DoorsDoors, DateTime.Now);
                var price = GetDocumentPrice(senderCity.Ref, recipientCity.Ref, enNovaPoshtaDeliveryType.DoorsDoors, TotalWeight, Rate != 0 ? TotalPrice / Rate : TotalPrice);
                var days = (delivery - DateTime.Today).Days;
                return new ShippingOption
                {
                    DeliveryTime = days + " " + Strings.Numerals(days, "нет", "день", "дня", "дней"),
                    Rate = price * Rate
                };
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return null;
            }
        }

        private float GetDocumentPrice(Guid citySender, Guid cityRecipient, enNovaPoshtaDeliveryType serviceType, float weight, float coast)
        {
            var model = new NovaPoshtaRequest<NovaRequestPrice>
            {
                ApiKey = ApiKey,
                Language = Language,
                ModelName = "InternetDocument",
                CalledMethod = "getDocumentPrice",
                MethodProperties = new NovaRequestPrice
                {
                    CitySender = citySender,
                    CityRecipient = cityRecipient,
                    ServiceType = serviceType.ToString(),
                    Weight = weight,
                    Cost = coast
                }
            };
            var str = MakeRequest(JsonConvert.SerializeObject(model));
            var obj = JsonConvert.DeserializeObject<NovaResponse<NovaResponsePrice>>(str);
            if (obj.Errors != null && obj.Errors.ToString() != "[]") throw new Exception(obj.Errors.ToString());
            return obj.Data.First().Cost;
        }

        private DateTime GetDocumentDeliveryDate(Guid citySender, Guid cityRecipient, enNovaPoshtaDeliveryType serviceType, DateTime dateTime)
        {
            var model = new NovaPoshtaRequest<NovaRequestDelivery>
            {
                ApiKey = ApiKey,
                Language = Language,
                ModelName = "InternetDocument",
                CalledMethod = "getDocumentDeliveryDate",
                MethodProperties = new NovaRequestDelivery
                {
                    CitySender = citySender,
                    CityRecipient = cityRecipient,
                    ServiceType = serviceType.ToString(),
                    DateTime = dateTime.ToString("dd.MM.yyyy")
                }
            };
            var str = MakeRequest(JsonConvert.SerializeObject(model));
            var obj = JsonConvert.DeserializeObject<NovaResponse<NovaResponseDeliveryDate>>(str);
            if (obj.Errors != null && obj.Errors.ToString() != "[]") throw new Exception(obj.Errors.ToString());
            return obj.Data.First().DeliveryDate.Date;
        }

        private NovaResponseCity GetCity(string cityName, string area)
        {
            var citiesSt = GetCities(0, cityName);
            var cities = JsonConvert.DeserializeObject<NovaResponse<NovaResponseCity>>(citiesSt, new BoolConverter());
            if (cities.Errors != null && cities.Errors.ToString() != "[]") throw new Exception(cities.Errors.ToString());
            return cities.Data.Count > 1 ? FindCityByRegion(cities.Data, area) : cities.Data.First();
        }

        private NovaResponseCity FindCityByRegion(IEnumerable<NovaResponseCity> cities, string areaName)
        {
            var area = GetArea(areaName);
            var city = area == null ? cities.FirstOrDefault() : cities.FirstOrDefault(x => x.Area == area.Ref);
            if (city == null)
                throw new Exception("not found city");
            return city;
        }

        private NovaResponseArea GetArea(string areaName)
        {
            var areaSt = GetAreas(0);
            var areas = JsonConvert.DeserializeObject<NovaResponse<NovaResponseArea>>(areaSt);
            if (areas.Errors != null && areas.Errors.ToString() != "[]") throw new Exception(areas.Errors.ToString());
            return areas.Data.FirstOrDefault(x => x.Description == areaName);
        }

        private string GetAreas(int page, string @ref = "")
        {
            var model = new NovaPoshtaRequest<NovaRequestArea>
            {
                ApiKey = ApiKey,
                Language = Language,
                ModelName = "Address",
                CalledMethod = "getAreas",
                MethodProperties = new NovaRequestArea { Page = page, Ref = @ref }
            };
            return MakeRequest(JsonConvert.SerializeObject(model));
        }

        private string GetCities(int page, string findByString, string @ref = "")
        {
            var model = new NovaPoshtaRequest<NovaRequestCity>
            {
                ApiKey = ApiKey,
                Language = Language,
                ModelName = "Address",
                CalledMethod = "getCities",
                MethodProperties = new NovaRequestCity { Page = page, FindByString = findByString, Ref = @ref }
            };
            return MakeRequest(JsonConvert.SerializeObject(model));
        }

        private static string MakeRequest(string data)
        {
            try
            {
                var request = WebRequest.Create(Url) as HttpWebRequest;
                request.Method = "POST";
                request.ContentType = "application/json";

                if (data.IsNotEmpty())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    request.ContentLength = bytes.Length;

                    using (var requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(bytes, 0, bytes.Length);
                        requestStream.Close();
                    }
                }

                var responseContent = "";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                responseContent = reader.ReadToEnd();
                            }
                    }
                }

                return responseContent;
            }
            catch (WebException ex)
            {
                using (var eResponse = ex.Response)
                {
                    if (eResponse != null)
                    {
                        using (var eStream = eResponse.GetResponseStream())
                            if (eStream != null)
                                using (var reader = new StreamReader(eStream))
                                {
                                    var error = reader.ReadToEnd();

                                }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }
    }
}