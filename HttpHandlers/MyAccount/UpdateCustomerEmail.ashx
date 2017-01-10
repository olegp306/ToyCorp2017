<%@ WebHandler Language="C#" Class="UpdateCustomerEmail" %>


using System.Web;
using System.Web.SessionState;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using Newtonsoft.Json;

public class UpdateCustomerEmail : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        if (ValidationHelper.IsValidEmail(context.Request["email"]) && !CustomerService.ExistsEmail(context.Request["email"]) && CustomerContext.CurrentCustomer.EMail.Contains("@temp"))
        {
            CustomerService.UpdateCustomerEmail(CustomerContext.CustomerId, context.Request["email"]);

            var customer = CustomerService.GetCustomer(CustomerContext.CustomerId);
            AdvantShop.Security.AuthorizeService.SignIn(customer.EMail, customer.Password, true, true);
            
            context.Response.Write(JsonConvert.SerializeObject(true));
        }
        else
        {
            context.Response.Write(JsonConvert.SerializeObject(false));
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}
