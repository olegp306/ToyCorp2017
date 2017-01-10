using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Modules;
using AdvantShop.Orders;

namespace Advantshop.Modules.UserControls
{
    public partial class CartInfo : System.Web.UI.Page
    {
        protected CustomerContact ShippingContact;

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var customerId = Request["Id"].TryParseGuid();

            var customer = CustomerService.GetCustomer(customerId);
            var confirmCart = AbandonedCartsService.GetAbondonedCart(customerId);

            Title = "Временная корзина";

            if (confirmCart != null && confirmCart.OrderConfirmationData != null &&
                confirmCart.OrderConfirmationData.Customer != null)
            {
                customer = confirmCart.OrderConfirmationData.Customer;
                ShippingContact = confirmCart.OrderConfirmationData.ShippingContact;

                if (confirmCart.OrderConfirmationData.SelectedShippingItem != null)
                {
                    lblShippingMethodName.Text = confirmCart.OrderConfirmationData.SelectedShippingItem.MethodNameRate;
                }

                if (confirmCart.OrderConfirmationData.SelectedPaymentItem != null)
                {
                    lblPaymentMethodName.Text = confirmCart.OrderConfirmationData.SelectedPaymentItem.Name;
                }
            }
            else
            {
                customerInfo.Visible = false;
                divShipPayments.Visible = false;
                customerInfoNotExist.Visible = true;
            }
                

            if (customer != null)
            {
                if (customer.RegistredUser)
                {
                    lnkCustomerName.Text = customer.FirstName + " " + customer.LastName + " " + customer.Patronymic;
                    lnkCustomerName.NavigateUrl = @"viewcustomer.aspx?customerid=" + customer.Id;
                    lnkCustomerEmail.Text = customer.EMail;
                    lnkCustomerEmail.NavigateUrl = "mailto:" + customer.EMail;

                    lblCustomerName.Visible = false;
                    lblCustomerEmail.Visible = false;
                }
                else
                {
                    lblCustomerName.Text = customer.FirstName + " " + customer.LastName + " " + customer.Patronymic;
                    lblCustomerEmail.Text = customer.EMail;

                    lnkCustomerName.Visible = false;
                    lnkCustomerEmail.Visible = false;
                }
                lblCustomerPhone.Text = customer.Phone;
            }

            if (ShippingContact != null)
            {
                lblShippingCountry.Text = ShippingContact.Country;
                lblShippingCity.Text = ShippingContact.City;
                lblShippingRegion.Text = ShippingContact.RegionName;
                lblShippingZipCode.Text = ShippingContact.Zip;
                lblShippingAddress.Text = ShippingContact.Address;
            }

            var shoppingCart = ShoppingCartService.GetShoppingCart(ShoppingCartType.ShoppingCart, customerId);
            if (shoppingCart != null)
            {
                lvOrderItems.DataSource = shoppingCart;
                lvOrderItems.DataBind();
            }
        }

        protected string RenderPicture(int productId, int? photoId)
        {
            if (photoId != null)
            {
                var photo = PhotoService.GetPhoto((int)photoId);
                if (photo != null)
                {
                    return string.Format("<img src='{0}'/>", FoldersHelper.GetImageProductPath(ProductImageType.Small, photo.PhotoName, true));
                }
            }

            var p = ProductService.GetProduct(productId);
            if (p != null && p.Photo.IsNotEmpty())
            {
                return String.Format("<img src='{0}'/>", FoldersHelper.GetImageProductPath(ProductImageType.XSmall, p.Photo, true));
            }

            return string.Format("<img src='{0}' alt=\"\"/>", UrlService.GetAbsoluteLink("images/nophoto_xsmall.jpg"));
        }

        protected string RenderSelectedOptions(IList<EvaluatedCustomOptions> evlist)
        {
            if (evlist == null || evlist.Count == 0)
                return string.Empty;

            var html = new StringBuilder();

            foreach (EvaluatedCustomOptions ev in evlist)
            {
                html.Append(string.Format("<div>{0}: {1}</div>", ev.CustomOptionTitle, ev.OptionTitle));
            }

            return html.ToString();
        }
    }
}