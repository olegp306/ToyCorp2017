<%@ WebHandler Language="C#" Class="DeleteFromCart" %>

using System;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using AdvantShop;
using AdvantShop.Customers;
using AdvantShop.Orders;
using Newtonsoft.Json;

public class DeleteFromCart : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        if (context.Request["itemId"].IsInt())
        {
            int itemId = int.Parse(context.Request["itemId"]);
            var carts = ShoppingCartService.GetAllShoppingCarts(CustomerContext.CustomerId);
            if (carts.Any(item => item.ShoppingCartItemId == itemId))
            {
                ShoppingCartService.DeleteShoppingCartItem(itemId);
            }
            ResponseStatus(context, "success");
        }
        else
        {
            ResponseStatus(context, "fail");
        }
        
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
