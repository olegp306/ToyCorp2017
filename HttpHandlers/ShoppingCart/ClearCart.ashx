<%@ WebHandler Language="C#" Class="ClearCart" %>

using System.Web;
using AdvantShop.Orders;
using Newtonsoft.Json;

public class ClearCart : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        ShoppingCartService.ClearShoppingCart(ShoppingCartType.ShoppingCart);

        ResponseStatus(context, "success");
    }

    private void ResponseStatus(HttpContext context, string status)
    {
        var shpCart = ShoppingCartService.CurrentShoppingCart;

        context.Response.Write(JsonConvert.SerializeObject(new
        {
            TotalItems = shpCart.TotalItems,
            status
        }));
    }
    
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}