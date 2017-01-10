<%@ WebHandler Language="C#" Class="HttpHandlers.Details.WishListCheck" %>

using System;
using System.Linq;
using System.Web;
using AdvantShop;
using Newtonsoft.Json;

namespace HttpHandlers.Details
{
    public class WishListCheck : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            var offerId = context.Request["offerId"].TryParseInt();
            var result = false;

            if (offerId != 0)
            {
                result = AdvantShop.Orders.ShoppingCartService.CurrentWishlist.Any(item => item.OfferId == offerId);
            }

            context.Response.Write(JsonConvert.SerializeObject(new
            {
                isExist = result
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
}

