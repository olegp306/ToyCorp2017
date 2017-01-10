<%@ WebHandler Language="C#" Class="GetCities" %>

using System.Linq;
using System.Web;
using AdvantShop;
using AdvantShop.Repository;
using Newtonsoft.Json;

public class GetCities : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";


        var countryId = context.Request["countryId"].TryParseInt();

        var countries =
            CountryService.GetCountriesByDisplayInPopup().Select(item => new { item.CountryId, item.Name, item.Iso2 }).ToList();

        var cities = new object();
        var country = new Country();
        var zone = IpZoneContext.CurrentZone;

        if (countryId != 0)
        {
            cities = CityService.GetCitiesByCountryInPopup(countryId).Select(item => new { item.CityId, item.Name, item.RegionId }).ToList();
            country = CountryService.GetCountry(countryId);
        }
        else
        {
            cities = CityService.GetCitiesByCountryInPopup(zone.CountryId).Select(item => new { item.CityId, item.Name, item.RegionId }).ToList();
        }

        var obj = new
        {
            countries,
            cities,
            current = new
            {
                CountryId = countryId != 0 ? country.CountryId : zone.CountryId,
                CountryName = countryId != 0 ? country.Name : zone.CountryName,
                zone.RegionId,
                zone.Region,
                zone.CityId,
                zone.City
            }
        };

        context.Response.Write(JsonConvert.SerializeObject(obj));
    }

    public bool IsReusable
    {
        get { return false; }
    }
}