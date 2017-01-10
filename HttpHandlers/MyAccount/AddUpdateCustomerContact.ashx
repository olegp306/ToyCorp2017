<%@ WebHandler Language="C#" Class="AddUpdateCustomerContact" %>

using System;
using System.Web;
using System.Web.SessionState;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Repository;

public class AddUpdateCustomerContact : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        var valid = true;
        valid &= context.Request["fio"].IsNotEmpty();
        
        if (SettingsOrderConfirmation.IsShowCountry && SettingsOrderConfirmation.IsRequiredCountry)
        {
            valid &= context.Request["countryId"].IsNotEmpty() && context.Request["countryId"].IsInt();
            valid &= context.Request["country"].IsNotEmpty();
        }

        if (SettingsOrderConfirmation.IsShowState && SettingsOrderConfirmation.IsRequiredState)
        {
            valid &= context.Request["region"].IsNotEmpty();
        }

        if (SettingsOrderConfirmation.IsShowCity && SettingsOrderConfirmation.IsRequiredCity)
        {
            valid &= context.Request["city"].IsNotEmpty();
        }
        
        if (SettingsOrderConfirmation.IsShowAddress && SettingsOrderConfirmation.IsRequiredAddress)
        {
            valid &= context.Request["address"].IsNotEmpty();
        }

        if (!valid || !CustomerContext.CurrentCustomer.RegistredUser)
        {
            ReturnResult(context, false);
        }

        var ipZone = IpZoneContext.CurrentZone;
        
        var contact = context.Request["contactid"].IsNullOrEmpty()
                            ? new CustomerContact()
                            : CustomerService.GetCustomerContact(HttpUtility.HtmlDecode(context.Request["contactid"]));

        contact.Name = context.Request["fio"];
        contact.City = context.Request["city"].IsNotEmpty() ? context.Request["city"] : ipZone.City;
        contact.Address = context.Request["address"] ?? string.Empty;
        contact.Zip = context.Request["zip"] ?? string.Empty;

        var country = CountryService.GetCountry(context.Request["countryId"].TryParseInt());
        contact.CountryId = country != null ? country.CountryId : ipZone.CountryId;
        contact.Country = country != null ? country.Name : ipZone.CountryName;

        var regionId = RegionService.GetRegionIdByName(HttpUtility.HtmlDecode(context.Request["region"]));
        contact.RegionId = regionId != 0 ? regionId : ipZone.RegionId;
        contact.RegionName = context.Request["region"].IsNotEmpty() ? context.Request["region"] : ipZone.Region;
        
        if (context.Request["contactid"].IsNullOrEmpty())
        {
            var id = CustomerService.AddContact(contact, CustomerContext.CurrentCustomer.Id);
            ReturnResult(context, id != Guid.Empty);
        }
        else
        {
            CustomerService.UpdateContact(contact);
            ReturnResult(context, true);
        }
    }

    private static void ReturnResult(HttpContext context, bool result)
    {
        context.Response.ContentType = "application/json";
        context.Response.Write(result ? Newtonsoft.Json.JsonConvert.True : Newtonsoft.Json.JsonConvert.False);
        context.Response.End();
    }

    public bool IsReusable
    {
        get { return false; }
    }
}
