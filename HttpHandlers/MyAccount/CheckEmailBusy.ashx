<%@ WebHandler Language="C#" Class="CheckEmailBusy" %>


using System.Web;
using System.Web.SessionState;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using Newtonsoft.Json;

public class CheckEmailBusy : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        if (ValidationHelper.IsValidEmail(context.Request["email"]) && !CustomerService.ExistsEmail(context.Request["email"]))
        {
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
