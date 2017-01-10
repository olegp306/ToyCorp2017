<%@ WebHandler Language="C#" Class="DeleteProduct" %>

using System;
using System.Web;
using System.Web.SessionState;
using AdvantShop.Orders;
using Newtonsoft.Json;

public class DeleteProduct : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        if (string.IsNullOrEmpty(context.Request["offerid"]))
        {
            return;
        }

        int offerid = 0;
        Int32.TryParse(context.Request["offerid"], out offerid);

        if (offerid != 0)
        {
            var compareCart = ShoppingCartService.CurrentCompare;
            ShoppingCartItem item;
            if ((item = compareCart.Find(p => p.OfferId == offerid)) != null)
            {
                ShoppingCartService.DeleteShoppingCartItem(item.ShoppingCartItemId);
                ReturnResult(context, new {
                    compare = ShoppingCartService.CurrentCompare,
                    isComplete = true
                });
                return;
            }
        }

        ReturnResult(context, new
        {
            compare = ShoppingCartService.CurrentCompare,
            isComplete = false
        });
    }

    private static void ReturnResult(HttpContext context, object obj)
    {
        context.Response.ContentType = "application/json";
        context.Response.Write(JsonConvert.SerializeObject(obj));
        context.Response.End();
    }

    public bool IsReusable
    {
        get { return true;}
    }
}