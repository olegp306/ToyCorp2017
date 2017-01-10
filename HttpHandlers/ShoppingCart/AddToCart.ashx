<%@ WebHandler Language="C#" Class="HttpHandlers.ShoppingCart.AddToCart" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.Orders;
using AdvantShop.Helpers;
using Newtonsoft.Json;

namespace HttpHandlers.ShoppingCart
{
    public class AddToCart : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            AdvantShop.Localization.Culture.InitializeCulture();
            context.Response.ContentType = "application/json";

            Offer offer;

            if (context.Request["offerid"].IsNullOrEmpty())
            {
                var product = ProductService.GetProduct(context.Request["productid"].TryParseInt());
                if (product == null || product.Offers.Count == 0)
                {
                    ResponseStatus(context, "fail");
                    return;
                }
                if (!product.HasMultiOffer || product.Offers.Count == 1)
                {
                    offer = product.Offers.First();
                }
                else
                {
                    ResponseStatus(context, "redirect");
                    return;
                }
            }
            else
            {
                offer = OfferService.GetOffer(context.Request["offerid"].TryParseInt());
            }

            if (offer == null || !offer.Product.Enabled || !offer.Product.CategoryEnabled)
            {
                ResponseStatus(context, "fail");
                return;
            }
            
            IList<EvaluatedCustomOptions> listOptions = null;
            var selectedOptions = context.Request["AttributesXml"].IsNotEmpty() && context.Request["AttributesXml"] != "null"
                                                ? HttpUtility.UrlDecode(context.Request["AttributesXml"])
                                                : null;
            if (selectedOptions.IsNotEmpty())
            {
                try
                {
                    listOptions = CustomOptionsService.DeserializeFromXml(selectedOptions);
                }
                catch (Exception)
                {
                    listOptions = null;
                }
            }

            if (CustomOptionsService.DoesProductHaveRequiredCustomOptions(offer.ProductId) && listOptions == null)
            {
                ResponseStatus(context, "redirect");
                return;
            }

            float amount = context.Request["amount"].TryParseFloat(1);
            if (Single.IsNaN(amount))
                amount = offer.Product.MinAmount ?? 1;

            ShoppingCartService.AddShoppingCartItem(new ShoppingCartItem
                {
                    OfferId = offer.OfferId,
                    Amount = amount,
                    ShoppingCartType = ShoppingCartType.ShoppingCart,
                    AttributesXml = listOptions != null ? selectedOptions : string.Empty,
                });

            if (context.Request["payment"].IsNotEmpty() && context.Request["payment"].IsInt())
            {
                CommonHelper.SetCookie("payment", context.Request["payment"]);
            }

            foreach (var moduleShoppingCartPopup in AttachedModules.GetModules<IShoppingCartPopup>())
            {
                var classInstance = (IShoppingCartPopup)Activator.CreateInstance(moduleShoppingCartPopup, null);
                
                context.Response.Write(classInstance.GetShoppingCartPopupJson(offer, amount, false));
                return;
            }

            ResponseStatus(context, "success");
        }


        private void ResponseStatus(HttpContext context, string status)
        {
            var shpCart = ShoppingCartService.CurrentShoppingCart;
            
            context.Response.Write(JsonConvert.SerializeObject(new {
                shpCart.TotalItems,
                status
            }));
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
