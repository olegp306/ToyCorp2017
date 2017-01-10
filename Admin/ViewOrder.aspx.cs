//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using AdvantShop.BonusSystem;
using Resources;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Shipping;
using AdvantShop.Controls;
using AdvantShop;

namespace Admin
{
    public partial class ViewOrder : AdvantShopAdminPage
    {
        protected int OrderId = 0;
        protected string OrderNumber;
        protected bool IsPaid;
        protected float CurrencyValue;
        protected string CurrencyCode;

        protected Order order;

        protected bool ShippingTypeIsCdek = false;
        protected bool ShippingTypeIsCheckout = false;
        protected OrderPickPoint OrderPickPoint;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            if (!int.TryParse(Request["orderid"], out OrderId))
            {
                OrderId = OrderService.GetLastOrderId();

            }
            SetMeta(string.Format("{0} - {1} {2}", SettingsMain.ShopName, Resource.Admin_ViewOrder_ItemNum, OrderId));
            LoadOrder();
        }

        private void LoadOrder()
        {
            order = OrderService.GetOrder(OrderId);
            if (order == null)
                Response.Redirect("OrderSearch.aspx");

            lnkExportToExcel.NavigateUrl = "HttpHandlers/ExportOrderExcel.ashx?OrderID=" + order.OrderID;
            lnkEditOrder.NavigateUrl = "EditOrder.aspx?OrderID=" + order.OrderID;
            
            OrderNumber = order.Number;
            lblOrderId.Text = order.OrderID.ToString();
            lblOrderDate.Text = AdvantShop.Localization.Culture.ConvertDate(order.OrderDate);
            lblOrderNumber.Text = order.Number;

            IsPaid = order.PaymentDate != null && order.PaymentDate != DateTime.MinValue;

            if (order.OrderCurrency != null)
            {
                CurrencyValue = order.OrderCurrency.CurrencyValue;
                CurrencyCode = order.OrderCurrency.CurrencyCode;
            }

            if (order.OrderCustomer != null)
            {
                var customer = CustomerService.GetCustomer(order.OrderCustomer.CustomerID);
                if (customer != null && customer.Id != Guid.Empty)
                {
                    lnkCustomerName.Text = order.OrderCustomer.FirstName + @" " + order.OrderCustomer.LastName;
                    lnkCustomerName.NavigateUrl = @"viewcustomer.aspx?customerid=" + order.OrderCustomer.CustomerID;
                    lnkCustomerEmail.Text = order.OrderCustomer.Email;
                    lnkCustomerEmail.NavigateUrl = "mailto:" + order.OrderCustomer.Email;
                }
                else
                {
                    lblCustomerEmail.Text = order.OrderCustomer.Email;
                    lblCustomerName.Text = order.OrderCustomer.FirstName + @" " + order.OrderCustomer.LastName;
                }

                lblCustomerPhone.Text = order.OrderCustomer.MobilePhone;
            }

            if (order.ShippingContact != null)
            {
                lblShippingCountry.Text = order.ShippingContact.Country;
                lblShippingCity.Text = order.ShippingContact.City;
                lblShippingRegion.Text = order.ShippingContact.Zone;
                lblShippingZipCode.Text = order.ShippingContact.Zip;
                lblShippingAddress.Text = order.ShippingContact.Address;

                if (!string.IsNullOrEmpty(order.ShippingContact.Country) && !string.IsNullOrEmpty(order.ShippingContact.City)
                    && !string.IsNullOrEmpty(order.ShippingContact.Zone) && !string.IsNullOrEmpty(order.ShippingContact.Address))
                {
                    lnkShippingAddressOnMap.NavigateUrl = (SettingsOrderConfirmation.PrintOrder_MapType == "googlemap"
                                                                ? "https://maps.google.com/maps?ie=UTF8&z=15&q="
                                                                : "http://maps.yandex.ru/?text=") +
                                                            HttpUtility.UrlEncode(order.ShippingContact.Country + "," + order.ShippingContact.Zone + "," +
                                                                                order.ShippingContact.City + "," + order.ShippingContact.Address);
                }
                else
                {
                    lnkShippingAddressOnMap.Visible = false;
                }
            }

            if (order.BillingContact != null)
            {
                lblBuyerCountry.Text = order.BillingContact.Country;
                lblBuyerRegion.Text = order.BillingContact.Zone;
                lblBuyerCity.Text = order.BillingContact.City;
                lblBuyerZip.Text = order.BillingContact.Zip;
                lblBuyerAddress.Text = order.BillingContact.Address;

                if (!string.IsNullOrEmpty(order.BillingContact.Country) && !string.IsNullOrEmpty(order.BillingContact.City)
                    && !string.IsNullOrEmpty(order.BillingContact.Zone) && !string.IsNullOrEmpty(order.BillingContact.Address))
                {
                    lnkBuyerAddressOnMap.NavigateUrl = (SettingsOrderConfirmation.PrintOrder_MapType == "googlemap"
                                                            ? "https://maps.google.com/maps?ie=UTF8&z=15&q="
                                                            : "http://maps.yandex.ru/?text=") +
                                                        HttpUtility.UrlEncode(order.BillingContact.Country + "," + order.BillingContact.Zone + "," +
                                                                                order.BillingContact.City + "," + order.BillingContact.Address);
                }
                else
                {
                    lnkBuyerAddressOnMap.Visible = false;
                }
            }

            lblShippingMethodName.Text = order.ArchivedShippingName + (order.OrderPickPoint != null ? "<br />" + order.OrderPickPoint.PickPointAddress : "");
            lblPaymentMethodName.Text = order.PaymentMethodName;

            var statusesList = OrderService.GetOrderStatuses();
            if (statusesList != null && statusesList.Any(status => status.StatusID == order.OrderStatus.StatusID))
            {
                ddlViewOrderStatus.DataSource = statusesList;
                ddlViewOrderStatus.DataBind();
                ddlViewOrderStatus.SelectedValue = order.OrderStatus.StatusID.ToString();
            }
            else
            {
                ddlViewOrderStatus.Items.Add(new ListItem(order.OrderStatus.StatusName, order.OrderStatus.StatusID.ToString()));
                ddlViewOrderStatus.SelectedValue = order.OrderStatus.StatusID.ToString();
            }
            ddlViewOrderStatus.Attributes["data-orderid"] = order.OrderID.ToString();

            pnlOrderNumber.Attributes["style"] = "border-left-color: #" + order.OrderStatus.Color;

            if (order.OrderCertificates == null || order.OrderCertificates.Count == 0)
            {
                lvOrderItems.DataSource = order.OrderItems;
                lvOrderItems.DataBind();
                lvOrderCertificates.Visible = false;
            }
            else
            {
                lvOrderCertificates.DataSource = order.OrderCertificates;
                lvOrderCertificates.DataBind();
                lvOrderItems.Visible = false;
            }

            lblUserComment.Text = string.IsNullOrEmpty(order.CustomerComment)
                                        ? Resource.Admin_OrderSearch_NoComment
                                        : order.CustomerComment;

            txtAdminOrderComment.Text = string.Format("{0}", order.AdminOrderComment);
            txtStatusComment.Text = string.Format("{0}", order.StatusComment);

            txtStatusComment.Attributes["data-orderid"] = order.OrderID.ToString();
            txtAdminOrderComment.Attributes["data-orderid"] = order.OrderID.ToString();

            var shipping = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            if (shipping != null)
            {
                OrderPickPoint = order.OrderPickPoint;

                liMultiship.Visible = shipping.Type == ShippingType.Multiship;

                liSendBillingLink.Visible =  order.OrderCustomer != null && order.ShippingMethod != null && !order.Payed;

                ShippingTypeIsCdek = shipping.Type == ShippingType.Cdek;

                ShippingTypeIsCheckout = lblCheckoutAdressNotice.Visible = shipping.Type == ShippingType.CheckoutRu;

                if (CustomerContext.CurrentCustomer.CustomerRole == Role.Moderator)
                {
                    var actions = RoleActionService.GetCustomerRoleActionsByCustomerId(CustomerContext.CurrentCustomer.Id);
                    bool showSendPaymentLink =
                        actions.Any(a => a.Key == RoleActionKey.DisplaySendPaymentLink && a.Enabled);

                    liSendBillingLink.Visible &= showSendPaymentLink;
                }

            }
            
            if (BonusSystem.IsActive)
            {
                var purchase = BonusSystemService.GetPurchase(order.Number, order.OrderID);
                if (purchase != null)
                {
                    bonusCardBlock.Visible = true;
                    lblBonusCardNumber.Text = purchase.CardNumber;
                    lblBonusCardAmount.Text = purchase.NewBonusAmount.ToString();
                }
            }

            if (Settings1C.Enabled)
            {
                divUseIn1C.Visible = true;
                chkUseIn1C.Checked = order.UseIn1C;
                chkUseIn1C.Attributes["data-orderid"] = order.OrderID.ToString();

                var status1C = OrderService.GetStatus1C(order.OrderID);
                if (status1C != null)
                {
                    divStatus1C.Visible = true;
                    lbl1CStatus.Text = status1C.Status1C;
                }
            }
            else
            {
                divUseIn1C.Visible = false;
            }

            LoadTotal(order);
        }

        private void LoadTotal(Order order)
        {
            lblShippingPrice.Text = string.Format("{0}{1}", order.ShippingCost > 0 ? "+" : "", CatalogService.GetStringPrice(order.ShippingCost, CurrencyValue, CurrencyCode));

            lvTaxes.DataSource = order.Taxes;
            lvTaxes.DataBind();

            float prodTotal = 0;
            if (order.OrderCertificates != null && order.OrderCertificates.Count > 0)
            {
                prodTotal = order.OrderCertificates.Sum(item => item.Sum);
            }
            else
            {
                prodTotal = order.OrderItems.Sum(oi => oi.Price * oi.Amount);
            }

            float totalDiscount = 0;

            totalDiscount += order.OrderDiscount > 0 ? Convert.ToSingle(Math.Round(prodTotal / 100 * order.OrderDiscount, 2)) : 0;

            lblTotalOrderPrice.Text = CatalogService.GetStringPrice(prodTotal, CurrencyValue, CurrencyCode);

            lblOrderDiscount.Text = string.Format("-{0}", CatalogService.GetStringPrice(totalDiscount, CurrencyValue, CurrencyCode));
            lblOrderDiscountPercent.Text = order.OrderDiscount + @"%";
            trDiscount.Visible = order.OrderDiscount != 0;

            lblOrderBonuses.Text = string.Format("-{0}", CatalogService.GetStringPrice(order.BonusCost, CurrencyValue, CurrencyCode));
            trBonuses.Visible = order.BonusCost != 0;

            liPaymentPrice.Visible = order.PaymentCost != 0;
            lblPaymentPrice.Text = (order.PaymentCost > 0 ? "+" : "") + CatalogService.GetStringPrice(order.PaymentCost, CurrencyValue, CurrencyCode);

            if (order.Certificate != null)
            {
                trCertificatePrice.Visible = order.Certificate.Price != 0;
                lblCertificatePrice.Text = string.Format("-{0}", CatalogService.GetStringPrice(order.Certificate.Price));
                totalDiscount += order.Certificate.Price;
            }

            if (order.Coupon != null)
            {
                float couponValue;
                trCoupon.Visible = order.Coupon.Value != 0;
                switch (order.Coupon.Type)
                {
                    case CouponType.Fixed:
                        var productsPrice =
                            order.OrderItems.Where(p => p.IsCouponApplied).Sum(p => p.Price * p.Amount);
                        couponValue = productsPrice >= order.Coupon.Value ? order.Coupon.Value : productsPrice;
                        totalDiscount += couponValue;
                        lblCoupon.Text = String.Format("-{0} ({1})", CatalogService.GetStringPrice(couponValue, CurrencyValue, CurrencyCode),
                                                       order.Coupon.Code);
                        break;
                    case CouponType.Percent:
                        couponValue = order.OrderItems.Where(p => p.IsCouponApplied).Sum(p => order.Coupon.Value * p.Price / 100 * p.Amount);
                        totalDiscount += couponValue;

                        lblCoupon.Text = String.Format("-{0} ({1}%) ({2})", CatalogService.GetStringPrice(couponValue, CurrencyValue, CurrencyCode),
                                                       CatalogService.FormatPriceInvariant(order.Coupon.Value),
                                                       order.Coupon.Code);
                        break;
                }
            }

            float sum = prodTotal - totalDiscount - order.BonusCost + order.ShippingCost + order.Taxes.Where(tax => !tax.TaxShowInPrice).Sum(tax => tax.TaxSum) + order.PaymentCost;
            lblTotalPrice.Text = CatalogService.GetStringPrice(sum < 0 ? 0 : sum, CurrencyValue, CurrencyCode);
        }

        protected string RenderSelectedOptions(IList<EvaluatedCustomOptions> evlist)
        {
            if (evlist.Count == 0)
            {
                return string.Empty;
            }

            var html = new StringBuilder();

            foreach (EvaluatedCustomOptions ev in evlist)
            {
                html.Append(string.Format("<div>{0}: {1}</div>", ev.CustomOptionTitle, ev.OptionTitle));
            }

            return html.ToString();
        }

        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            if (int.TryParse(Request["orderid"], out OrderId))
            {
                OrderService.DeleteOrder(OrderId);
                Response.Redirect("ordersearch.aspx");
            }
        }

        protected string RenderPicture(int productId, int? photoId)
        {
            if (photoId != null)
            {
                var photo = PhotoService.GetPhoto((int)photoId);
                if (photo != null)
                {
                    return string.Format("<img src='{0}'/>", FoldersHelper.GetImageProductPath(ProductImageType.XSmall, photo.PhotoName, true));
                }
            }

            var p = ProductService.GetProduct(productId);
            if (p != null && p.Photo.IsNotEmpty())
            {
                return String.Format("<img src='{0}'/>", FoldersHelper.GetImageProductPath(ProductImageType.XSmall, p.Photo, true));
            }

            return string.Format("<img src='{0}' alt=\"\"/>", AdvantShop.Core.UrlRewriter.UrlService.GetAbsoluteLink("images/nophoto_xsmall.jpg"));
        }

        protected string RenderPaidButtons()
        {
            return
                string.Format(
                    "<label><input type=\"radio\" {0} name=\"g-checkout\" value=\"1\" onclick=\"setOrderPaid(1,{2})\"/>" + Resource.Admin_ViewOrder_Paid + "</label>" +
                    "<label><input type=\"radio\" {1} name=\"g-checkout\" value=\"0\" onclick=\"setOrderPaid(0,{2})\"/>" + Resource.Admin_ViewOrder_NotPaid + "</label>",
                    IsPaid ? "checked=\"checked\"" : string.Empty,
                    !IsPaid ? "checked=\"checked\"" : string.Empty,
                    Request["orderid"]);
        }
    }
}