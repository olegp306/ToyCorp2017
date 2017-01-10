<%@ WebHandler Language="C#" Class="HttpHandlers.Details.AddToWishlist" %>

//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Globalization;
using System.Web;
using System.Web.SessionState;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace HttpHandlers.Details
{
    public class AddToWishlist : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            var offerId = 0;

            var valid = true;
            valid &= context.Request["offerid"].IsNotEmpty() && Int32.TryParse(context.Request["offerid"], out offerId);

            if (!valid)
            {
                ReturnResult(context, new
                {
                    Count = 0,
                    CountString = Resources.Resource.Client_MasterPage_WishList_Empty
                });
            }

            string options = HttpUtility.UrlDecode(context.Request["customOptions"]);

            var offer = OfferService.GetOffer(offerId);
            if (offer != null)
            {
                ShoppingCartService.AddShoppingCartItem(new ShoppingCartItem
                    {
                        OfferId = offer.OfferId,
                        ShoppingCartType = ShoppingCartType.Wishlist,
                        AttributesXml = options,
                    });
            }
            else
            {
                ReturnResult(context, new
                {
                    Count = 0,
                    CountString = Resources.Resource.Client_MasterPage_WishList_Empty
                });

            }

            int wishCount = ShoppingCartService.CurrentWishlist.Count;
            string wishlistCount = string.Format("{0} {1}",
                                                 wishCount == 0 ? "" : wishCount.ToString(CultureInfo.InvariantCulture),
                                                 Strings.Numerals(wishCount,
                                                                  Resources.Resource.Client_MasterPage_WishList_Empty,
                                                                  Resources.Resource.Client_MasterPage_WishList_1Product,
                                                                  Resources.Resource
                                                                           .Client_MasterPage_WishList_2Products,
                                                                  Resources.Resource
                                                                           .Client_MasterPage_WishList_5Products));




            ReturnResult(context, new
            {
                Count = wishCount,
                CountString = wishlistCount
            });
        }

        private static void ReturnResult(HttpContext context, object obj)
        {
            context.Response.Write(JsonConvert.SerializeObject(obj));
            context.Response.End();
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