using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using AdvantShop.Diagnostics;

namespace AdvantShop.Repository
{
    public class GeoIpService
    {
        private const string GeoServiceUrl = "http://ipgeobase.ru:7020/geo?ip=";

        /// <summary>
        /// Get city name by ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static GeoIpData GetGeoIpData(string ip)
        {
            try
            {
                var request = WebRequest.Create(GeoServiceUrl + ip);
                request.Timeout = 300;

                using (var dataStream = request.GetResponse().GetResponseStream())
                using (var reader = new StreamReader(dataStream, Encoding.GetEncoding("Windows-1251")))
                {
                    var responseFromServer = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(responseFromServer))
                    {
                        var ipElement = XElement.Parse(responseFromServer).Element("ip");
                        if (ipElement != null)
                        {
                            var countryIso = ipElement.Element("country");
                            var city = ipElement.Element("city");
                            var state = ipElement.Element("region");

                            if (countryIso != null)
                                return new GeoIpData()
                                {
                                    Country = countryIso.Value,
                                    City = city != null ? city.Value : string.Empty,
                                    State = state != null ? state.Value : string.Empty
                                };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex,false);
            }

            return null;
        }
    }
}