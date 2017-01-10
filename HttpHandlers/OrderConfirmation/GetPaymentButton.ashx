<%@ WebHandler Language="C#" Class="GetPaymentButton" %>


using System.Web;
using System.Web.SessionState;
using AdvantShop.Configuration;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Payment;
using Newtonsoft.Json;

public class GetPaymentButton : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        AdvantShop.Localization.Culture.InitializeCulture();
        context.Response.ContentType = "application/json";
        if (string.IsNullOrEmpty(context.Request["orderid"]))
        {
            context.Response.Write(JsonConvert.SerializeObject(string.Empty));
            return;
        }
        var order = OrderService.GetOrder(SQLDataHelper.GetInt(context.Request["orderid"]));
        var script = OrderService.ProcessOrder(order, PaymentService.PageWithPaymentButton.orderconfirmation);
        if (script.Length > 0)
        {
            context.Response.Write(JsonConvert.SerializeObject(new
            {
                formString = script.Contains("</form>") ? script.Substring(0, script.IndexOf("</form>") + 7) : "",
                buttonString = script.Contains("</form>") ? script.Substring(script.IndexOf("</form>") + 7, script.Length - (script.IndexOf("</form>") + 7)) : script,
                proceedToPayment = SettingsOrderConfirmation.ProceedToPayment
            }));
        }
        else
        {
            context.Response.Write(script);
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }
}
