<%@ WebHandler Language="C#" Class="OfferFirstPaymentPrice" %>

using System.Web;
using AdvantShop;
using AdvantShop.Catalog;
using Newtonsoft.Json;

public class OfferFirstPaymentPrice : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        if (string.IsNullOrEmpty(context.Request["price"]))
        {
            context.Response.Write(string.Empty);
            return;
        }
        
        string result = string.Empty;
        var price = context.Request["price"].Replace(".", ",").TryParseFloat();
        var discount = context.Request["discount"].Replace(".", ",").TryParseFloat();
        var firstPaymentPercent = context.Request["firstPaymentPercent"].Replace(".", ",").TryParseFloat();
        var minPrice = context.Request["minPrice"].Replace(".", ",").TryParseFloat();
        
        float discountByTime = DiscountByTimeService.GetDiscountByTime();

        var finalPrice = price - (price * (discount == 0 ? discountByTime : discount) / 100);

        if (price > minPrice)
        {
            if (firstPaymentPercent > 0)
            {
                result = CatalogService.GetStringPrice(finalPrice * firstPaymentPercent / 100) + "*";
            }
            else
            {
                result = string.Format("<div class=\"price\">{0}*</div>", Resources.Resource.Client_Details_WithoutFirstPayment);
            }
        }
        
        context.Response.Write(JsonConvert.SerializeObject(new { Price = result}));
    }  

    public bool IsReusable
    {
        get { return false; }
    }
}