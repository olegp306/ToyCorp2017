<%@ WebHandler Language="C#" Class="DeleteCustomerContacts" %>

using System;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using AdvantShop;
using AdvantShop.Customers;

public class DeleteCustomerContacts : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        if (!CustomerContext.CurrentCustomer.RegistredUser)
        {
            ReturnResult(context, false);
        }
        Guid id = context.Request["contactid"].TryParseGuid();
        if (id != Guid.Empty && CustomerContext.CurrentCustomer.Contacts.Any(contact => contact.CustomerContactID == id))
        {
            CustomerService.DeleteContact(id);
        }
        ReturnResult(context, true);
    }

    private static void ReturnResult(HttpContext context, bool result)
    {
        context.Response.ContentType = "application/json";
        context.Response.Write(result ? Newtonsoft.Json.JsonConvert.True : Newtonsoft.Json.JsonConvert.False);
        context.Response.End();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}
