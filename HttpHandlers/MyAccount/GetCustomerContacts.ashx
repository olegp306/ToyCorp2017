<%@ WebHandler Language="C#" Class="GetCustomerContacts" %>

using System.Linq;
using System.Web;
using System.Web.SessionState;
using AdvantShop.Customers;
using Newtonsoft.Json;

public class GetCustomerContacts : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        if(!CustomerContext.CurrentCustomer.RegistredUser)
        {
            context.Response.Write(JsonConvert.SerializeObject(string.Empty));
        }
        
        context.Response.ContentType = "application/json";

        var customerContacts = from item in CustomerContext.CurrentCustomer.Contacts
                               select new
                               {
                                   Country = HttpUtility.HtmlEncode(item.Country),
                                   City = HttpUtility.HtmlEncode(item.City),
                                   Address = HttpUtility.HtmlEncode(item.Address),
                                   Name = HttpUtility.HtmlEncode(item.Name),
                                   item.CountryId,
                                   item.RegionId,
                                   RegionName = HttpUtility.HtmlEncode(item.RegionName),
                                   Zip = HttpUtility.HtmlEncode(item.Zip),
                                   item.CustomerContactID
                               };

        context.Response.Write(JsonConvert.SerializeObject(customerContacts));
    }

    public bool IsReusable
    {
        get { return false; }
    }
}