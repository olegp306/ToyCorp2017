//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Controls;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;
using Resources;
using AdvantShop.Configuration;

namespace ClientPages
{
    public partial class PrintOrder : AdvantShopClientPage
    {
        #region Fields

        protected bool ShowStatusInfo = SettingsOrderConfirmation.PrintOrder_ShowStatusInfo;
        protected bool ShowMap = SettingsOrderConfirmation.PrintOrder_ShowMap;
        protected string MapType = SettingsOrderConfirmation.PrintOrder_MapType;
        protected string MapAdress;

        protected Order Order;
        protected OrderCurrency OrdCurrency = null;

        #endregion
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["ordernumber"].IsNullOrEmpty())
            {
                Response.Redirect("default.aspx");
                return;
            }

            try
            {
                var orderId = OrderService.GetOrderIdByNumber(Request["ordernumber"]);

                lblOrderID.Text = Resource.Admin_ViewOrder_ItemNum + orderId;

                Order = OrderService.GetOrder(orderId);

                if (Order == null)
                {
                    Response.Redirect("default.aspx");
                    return;
                }

                OrdCurrency = Order.OrderCurrency;
                lOrderDate.Text = AdvantShop.Localization.Culture.ConvertDate(Order.OrderDate);

                if (Order.OrderCertificates != null && Order.OrderCertificates.Count > 0)
                {
                    trBilling.Visible = false;
                    trShipping.Visible = false;
                }


                var productPrice = Order.OrderCertificates != null && Order.OrderCertificates.Count > 0
                                            ? Order.OrderCertificates.Sum(item => item.Sum)
                                            : Order.OrderItems.Sum(item => item.Amount*item.Price);

                var totalDiscount = Order.OrderDiscount;

                lblProductPrice.Text = CatalogService.GetStringPrice(productPrice, OrdCurrency.CurrencyValue,
                                                                        OrdCurrency.CurrencyCode);

                trDiscount.Visible = Order.OrderDiscount != 0;
                lblOrderDiscount.Text = string.Format("-{0}",
                                        CatalogService.GetStringDiscountPercent(productPrice, totalDiscount,
                                            OrdCurrency.CurrencyValue,
                                            Order.OrderCurrency.CurrencySymbol, Order.OrderCurrency.IsCodeBefore,
                                            CurrencyService.CurrentCurrency.PriceFormat, false));

                trBonus.Visible = Order.BonusCost != 0;
                lblOrderBonus.Text = string.Format("-{0}",
                                       CatalogService.GetStringPrice(Order.BonusCost,
                                           Order.OrderCurrency.CurrencyValue, Order.OrderCurrency.CurrencyCode));

                if (Order.Certificate != null)
                {
                    trCertificate.Visible = true;
                    lblCertificate.Text = string.Format("-{0}",
                                                        CatalogService.GetStringPrice(Order.Certificate.Price,
                                                                                        OrdCurrency.CurrencyValue,
                                                                                        OrdCurrency.CurrencyCode));
                }
                else
                {
                    trCertificate.Visible = false;
                }

                if (Order.Coupon != null)
                {
                    var productsWithCoupon = Order.OrderItems.Where(item => item.IsCouponApplied).Sum(item=> item.Price * item.Amount);
                    trCoupon.Visible = true;
                    switch (Order.Coupon.Type)
                    {
                        case CouponType.Fixed:
                            lblCoupon.Text = String.Format("-{0} ({1})",
                                                            CatalogService.GetStringPrice(Order.Coupon.Value, OrdCurrency.CurrencyValue, OrdCurrency.CurrencyCode),
                                                            Order.Coupon.Code);
                            break;
                        case CouponType.Percent:
                            lblCoupon.Text = String.Format("-{0} ({1}%) ({2})",
                                                            CatalogService.GetStringPrice(productsWithCoupon * Order.Coupon.Value / 100, OrdCurrency.CurrencyValue, OrdCurrency.CurrencyCode),
                                                            CatalogService.FormatPriceInvariant(Order.Coupon.Value), 
                                                            Order.Coupon.Code);
                            break;
                    }
                }
                else
                {
                    trCoupon.Visible = false;
                }

                lblShippingPrice.Text = string.Format("{0}{1}", Order.ShippingCost > 0 ? "+" : "",
                                        CatalogService.GetStringPrice(Order.ShippingCost, Order.OrderCurrency.CurrencyValue, Order.OrderCurrency.CurrencyCode));

                PaymentRow.Visible = Order.PaymentCost != 0;
                lblPaymentPrice.Text = string.Format("{0}{1}", Order.PaymentCost > 0 ? "+" : "",
                                       CatalogService.GetStringPrice(Order.PaymentCost, Order.OrderCurrency.CurrencyValue, Order.OrderCurrency.CurrencyCode));

                List<OrderTax> taxedItems = TaxServices.GetOrderTaxes(Order.OrderID);
                if (taxedItems.Count > 0)
                {
                    literalTaxCost.Text = TaxServices.BuildTaxTable(taxedItems, Order.OrderCurrency.CurrencyValue,
                                                                Order.OrderCurrency.CurrencyCode,
                                                                Resource.Admin_ViewOrder_Taxes);
                }
                

                lblTotalPrice.Text = CatalogService.GetStringPrice(Order.Sum, Order.OrderCurrency.CurrencyValue,
                                                                    Order.OrderCurrency.CurrencyCode);

                // ------------------------------------------------------------------------------------
                
                if (Order.OrderCertificates == null || Order.OrderCertificates.Count == 0)
                {
                    lvOrderItems.DataSource = Order.OrderItems;
                    lvOrderItems.DataBind();
                }
                else
                {
                    lvOrderGiftCertificates.DataSource = Order.OrderCertificates;
                    lvOrderGiftCertificates.DataBind();
                }


                MapAdress = Order.ShippingContact.Country + "," + Order.ShippingContact.Zone + "," +
                            Order.ShippingContact.City + "," + Order.ShippingContact.Address;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        protected string RenderSelectedOptions(IList<EvaluatedCustomOptions> evlist)
        {
            if (evlist == null || evlist.Count == 0)
                return "";

            var html = new StringBuilder();
            html.Append("<ul>");

            foreach (var ev in evlist)
            {
                html.Append(string.Format("<li>{0}: {1}</li>", ev.CustomOptionTitle, ev.OptionTitle));
            }

            html.Append("</ul>");
            return html.ToString();
        }
    }
}