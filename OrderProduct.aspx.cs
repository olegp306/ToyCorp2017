//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Controls;
using AdvantShop.Orders;

namespace ClientPages
{
    public partial class OrderProduct : AdvantShopClientPage
    {
        private string Code
        {
            get { return Request["code"]; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Code))
            {
                Response.Redirect("~/");
                return;
            }

            // Если код правильный, и такого же товара нет в корзине - то всё ок.
            var orderByRequest = OrderByRequestService.GetOrderByRequest(Code);

            if (orderByRequest == null)
            {
                lblMessage.Text = Resources.Resource.Client_OrderProduct_Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (ShoppingCartService.CurrentShoppingCart.Any(p => p.Offer.OfferId == orderByRequest.OfferId))
            {
                Response.Redirect("shoppingcart.aspx");
            }

            var offer = OfferService.GetOffer(orderByRequest.OfferId);
            if (orderByRequest != null && orderByRequest.IsValidCode && ProductService.IsExists(orderByRequest.ProductId) && offer != null && offer.Price > 0)
            {
                ShoppingCartService.AddShoppingCartItem(new ShoppingCartItem
                    {
                        OfferId = orderByRequest.OfferId,
                        Amount = orderByRequest.Quantity,
                        ShoppingCartType = ShoppingCartType.ShoppingCart,
                        AttributesXml = orderByRequest.Options
                    });
                Response.Redirect("shoppingcart.aspx");
            }
            else
            {
                lblMessage.Text = Resources.Resource.Client_OrderProduct_Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}