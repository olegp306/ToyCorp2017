using System;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Helpers;

namespace AdvantShop.Repository
{
    public static class IpZoneContext
    {
        private const string IpZoneContextKey = "IpZoneContext";
        private const string IpZoneCookie = "ipzone";

        public static IpZone CurrentZone
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    var contextZone = HttpContext.Current.Items[IpZoneContextKey] as IpZone;
                    if (contextZone != null) return contextZone;

                    var zone = GetZoneFromCookie();
                    if (zone == null)
                    {
                        zone = GetZoneByIp();
                        SetCustomerCookie(zone);
                    }

                    HttpContext.Current.Items[IpZoneContextKey] = zone;
                    return zone;
                }

                return GetDefaultZone();
            }
        }

        private static IpZone GetZoneFromCookie()
        {
            var cookie = HttpContext.Current.Request.Cookies[IpZoneCookie];

            if (cookie == null || cookie.Value.IsNullOrEmpty())
                return null;

            var cookieValues = HttpUtility.UrlDecode(cookie.Value).Split(new[] { ';' }, StringSplitOptions.None);
            if (cookieValues.Length != 5)
                return null;

            var countryId = cookieValues[0].TryParseInt();
            var regionId = cookieValues[1].TryParseInt();
            var cityId = cookieValues[2].TryParseInt();

            var region = cookieValues[3].Trim();
            var city = cookieValues[4].Trim();

            var country = CountryService.GetCountry(countryId);
            if (country == null || country.CountryId == 0)
                return null;

            if (cityId == 0 && city.IsNullOrEmpty())
                return null;

            if (regionId != 0)
            {
                var regionTemp = RegionService.GetRegion(regionId);
                regionId = regionTemp == null || regionTemp.RegionID == 0 ? 0 : regionTemp.RegionID;
                region = regionTemp == null || regionTemp.RegionID == 0 ? string.Empty : regionTemp.Name;
            }

            var zone = new IpZone()
            {
                CountryId = country.CountryId,
                CountryName = country.Name,
                RegionId = regionId,
                Region = region,
                CityId = cityId,
                City = city
            };

            return zone;
        }

        private static IpZone GetZoneByIp()
        {
            var zone = new IpZone();

            var geoIpData = GeoIpService.GetGeoIpData(HttpContext.Current.Request.UserHostAddress);
            if (geoIpData != null)
            {
                if (geoIpData.Country.IsNotEmpty())
                {
                    var country = CountryService.GetCountryByIso2(geoIpData.Country);
                    if (country != null)
                    {
                        zone.CountryId = country.CountryId;
                        zone.CountryName = country.Name;
                    }

                    zone.Region = geoIpData.State;
                    zone.City = geoIpData.City;
                }
            }

            if (zone.CountryId == 0)
            {
                zone.CountryId = SettingsMain.SellerCountryId;
                zone.CountryName = CountryService.GetCountry(zone.CountryId).Name;
            }

            if (zone.City.IsNullOrEmpty())
            {
                var state = RegionService.GetRegion(SettingsMain.SellerRegionId);
                if (state != null)
                {
                    var country = CountryService.GetCountry(state.CountryID);
                    zone.CountryId = state.CountryID;
                    zone.CountryName = country != null ? country.Name : string.Empty;
                    zone.Region = state.Name;
                    zone.RegionId = state.RegionID;
                }

                zone.City = SettingsMain.City;
            }
            else
            {
                var city = CityService.GetCityByName(zone.City);

                if (city != null)
                {
                    zone.CityId = city.CityId;

                    if (!CityService.IsCityInCountry(city.CityId, zone.CountryId))
                    {
                        var region = RegionService.GetRegion(city.RegionId);
                        if (region == null)
                            return GetDefaultZone();

                        var country = CountryService.GetCountry(region.CountryID);
                        if (country == null)
                            return GetDefaultZone();

                        zone.CountryId = country.CountryId;
                        zone.CountryName = country.Name;
                        zone.RegionId = region.RegionID;
                        zone.Region = region.Name;
                    }
                }
            }

            if (zone.Region.IsNullOrEmpty())
            {
                var state = RegionService.GetRegion(SettingsMain.SellerRegionId);
                if (state != null)
                {
                    zone.Region = state.Name;
                    zone.RegionId = state.RegionID;
                }
            }

            return zone;
        }

        private static IpZone GetDefaultZone()
        {
            return new IpZone()
            {
                CountryId = SettingsMain.SellerCountryId,
                CountryName = CountryService.GetCountry(SettingsMain.SellerCountryId).Name,
                RegionId = SettingsMain.SellerRegionId,
                Region = RegionService.GetRegion(SettingsMain.SellerRegionId).Name,
                City = SettingsMain.City
            };
        }

        public static void SetCustomerCookie(IpZone zone)
        {
            CommonHelper.SetCookie(IpZoneCookie,
                            string.Format("{0};{1};{2};{3};{4}", zone.CountryId, zone.RegionId, zone.CityId, zone.Region, zone.City),
                            new TimeSpan(365, 24, 0),
                            true);
        }

        public static bool ShowBubble()
        {
            return SettingsDesign.DisplayCityInTopPanel && SettingsDesign.DisplayCityBubble;
        }
    }
}