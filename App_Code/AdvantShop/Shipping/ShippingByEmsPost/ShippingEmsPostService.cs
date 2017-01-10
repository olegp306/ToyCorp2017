using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using AdvantShop.Core.Caching;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;

namespace AdvantShop.Shipping
{
    public static class ShippingEmsPostService
    {
        #region Fields

        private const string Url = "http://emspost.ru/api/rest/?";
        private const string CacheCitiesKey = "EmsPostCities";
        private const string CacheRegionsKey = "EmsPostRegions";
        private const string CacheCountriesKey = "EmsPostCountries";

        #endregion

        #region Help methods

        private static T GetRequest<T>(string requrl, string data = null) where T : class
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(Url + requrl);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "GET";

            if (data.IsNotEmpty())
            {
                var bytes = Encoding.UTF8.GetBytes(data);
                webRequest.ContentLength = bytes.Length;
                using (var requestStream = webRequest.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }
            }

            try
            {
                var responseContent = "";

                using (var response = webRequest.GetResponse())
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
                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return null;
        }
        
        public static float GetMaxWeight()
        {
            var response = GetRequest<EmsPostMaxWeightResponse>("method=ems.get.max.weight");
            return response != null && response.rsp.stat == "ok" ? response.rsp.max_weight : 0;
        }

        public static List<EmsPostLocation> GetCities()
        {
            if (CacheManager.Contains(CacheCitiesKey))
                return CacheManager.Get<List<EmsPostLocation>>(CacheCitiesKey);

            var response = GetRequest<EmsPostLocationsResponse>("method=ems.get.locations&type=cities&plain=true");
            if (response != null && response.rsp.stat == "ok" && response.rsp.locations.Any())
            {
                CacheManager.Insert(CacheCitiesKey, response.rsp.locations, 360);
                return response.rsp.locations;
            }

            return null;
        }

        public static List<EmsPostLocation> GetRegions()
        {
            if (CacheManager.Contains(CacheRegionsKey))
                return CacheManager.Get<List<EmsPostLocation>>(CacheRegionsKey);

            var response = GetRequest<EmsPostLocationsResponse>("method=ems.get.locations&type=regions&plain=true");
            if (response != null && response.rsp.stat == "ok" && response.rsp.locations.Any())
            {
                CacheManager.Insert(CacheRegionsKey, response.rsp.locations, 360);
                return response.rsp.locations;
            }

            return null;
        }

        public static List<EmsPostLocation> GetCountries()
        {
            if (CacheManager.Contains(CacheCountriesKey))
                return CacheManager.Get<List<EmsPostLocation>>(CacheCountriesKey);

            var response = GetRequest<EmsPostLocationsResponse>("method=ems.get.locations&type=countries&plain=true");
            if (response != null && response.rsp.stat == "ok" && response.rsp.locations.Any())
            {
                CacheManager.Insert(CacheCountriesKey, response.rsp.locations, 360);
                return response.rsp.locations;
            }

            return null;
        }

        private static EmsPostPrice GetPriceByLocation(List<EmsPostLocation> locations, string locationTo,
                                                            string cityFrom, float weight)
        {
            if (locations == null || locations.Count == 0)
                return null;

            var countryToValue = locations.FirstOrDefault(x => x.name.ToLower() == locationTo.ToLower());
            if (countryToValue == null)
                return null;

            var response =
                GetRequest<EmsPostPriceResponse>(
                    string.Format("method=ems.calculate&from={0}&to={1}&weight={2}&type=att",
                        cityFrom, countryToValue.value, weight.ToString("F2").Replace(",", ".")));

            if (response != null && response.rsp.stat != "ok" && response.rsp.err.IsNotEmpty())
            {
                Debug.LogError(new Exception(response.rsp.err), false);
            }

            return response != null && response.rsp.stat == "ok" ? response.rsp : null;
        }

        #endregion

        public static EmsPostPrice GetEmsPriceByCity(string cityFrom, string cityTo, float weigth)
        {
            return GetPriceByLocation(GetCities(), cityTo, cityFrom, weigth);
        }

        public static EmsPostPrice GetEmsPriceByRegion(string cityFrom, string regionTo, float weigth)
        {
            return GetPriceByLocation(GetRegions(), regionTo, cityFrom, weigth);
        }

        public static EmsPostPrice GetEmsPriceByCountry(string cityFrom, string countryTo, float weigth)
        {
            return GetPriceByLocation(GetCountries(), countryTo, cityFrom, weigth);
        }
    }
}