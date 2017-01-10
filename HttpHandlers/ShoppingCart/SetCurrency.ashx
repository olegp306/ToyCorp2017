<%@ WebHandler Language="C#" Class="AddToCart" %>
using System.Web;
using System.Web.SessionState;
using AdvantShop;
using AdvantShop.Repository.Currencies;

public class AddToCart : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        if (context.Request["ISO3"].IsNotEmpty())
        {
            CurrencyService.CurrentCurrency = CurrencyService.Currency(context.Request["ISO3"]);
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
