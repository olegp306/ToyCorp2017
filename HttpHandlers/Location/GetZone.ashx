<%@ WebHandler Language="C#" Class="GetZone" %>

using System.Web;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Repository;
using Newtonsoft.Json;

public class GetZone : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        if (context.Request["city"].IsNullOrEmpty())
            return;

        var zone = IpZoneService.GetZoneByCity(context.Request["city"].Trim().ToLower(), context.Request["countryID"].TryParseInt(true)) ??
                   new IpZone()
                   {
                       CountryId = SettingsMain.SellerCountryId,
                       CountryName = string.Empty,
                       Region = string.Empty,
                       City = HttpUtility.HtmlEncode(context.Request["city"].Trim())
                   };


        IpZoneContext.SetCustomerCookie(zone);

        var city = CityService.GetCityByName(zone.City);

        context.Response.Write(JsonConvert.SerializeObject(new IpZoneModel()
        {
            countryId = zone.CountryId,
            country = zone.CountryName,
            region = zone.Region,
            city = zone.City,
            cityId = zone.CityId,
            phone = city != null && city.PhoneNumber.IsNotEmpty() ? city.PhoneNumber : SettingsMain.Phone
        }));
    }

    public bool IsReusable
    {
        get { return false; }
    }
}