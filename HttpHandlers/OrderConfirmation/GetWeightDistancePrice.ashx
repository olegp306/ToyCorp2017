<%@ WebHandler Language="C#" Class="GetWeightDistancePrice" %>

using System.Linq;
using System.Web;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using Newtonsoft.Json;

public class GetWeightDistancePrice : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        if (string.IsNullOrEmpty(context.Request["distance"]) || !context.Request["distance"].IsInt())
        {
            context.Response.Write(string.Empty);
            return;
        }

        if (string.IsNullOrEmpty(context.Request["shipId"]) || !context.Request["shipId"].IsInt())
        {
            context.Response.Write(string.Empty);
            return;
        }

        var method = ShippingMethodService.GetShippingMethod(context.Request["shipId"].TryParseInt());

        if (method != null && method.Type == ShippingType.ShippingByRangeWeightAndDistance)
        {
            var methodParams = ShippingMethodService.GetShippingParams(method.ShippingMethodId);
            var rate = new ShippingByRangeWeightAndDistance(methodParams)
            {
                ShoppingCart = ShoppingCartService.CurrentShoppingCart
            }.GetRate(context.Request["distance"].TryParseInt());

            context.Response.Write(CatalogService.GetStringPrice(rate));
            return;
        }

        context.Response.Write(JsonConvert.SerializeObject(string.Empty));
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}