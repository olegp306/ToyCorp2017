<%@ WebHandler Language="C#" Class="Subscribe" %>

using System.Web;
using AdvantShop;
using AdvantShop.Customers;
using Newtonsoft.Json;
using Resources;

public class Subscribe : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        AdvantShop.Localization.Culture.InitializeCulture();
        context.Response.ContentType = "application/json";

        var email = context.Request["email"];

        if (email.IsNullOrEmpty())
        {
            ResponseStatus(context, "error", "not valid email");
            return;
        }

        if (SubscriptionService.IsSubscribe(email))
        {
            ResponseStatus(context, "error", Resource.Client_Subscribe_EmailAlreadyReg);
            return;
        }

        SubscriptionService.Subscribe(email);
        
        ResponseStatus(context, "success",
            Resource.Client_Subscribe_RegSuccess);
    }

    private void ResponseStatus(HttpContext context, string status, string text)
    {
        context.Response.Write(JsonConvert.SerializeObject(new
        {
            status,
            text
        }));
    }

    
    public bool IsReusable
    {
        get { return false; }
    }
}