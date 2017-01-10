//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Shipping;
using Resources;

namespace ClientPages
{
    public partial class Billing : Page
    {
        #region Fields

        private Order _order;

        protected float CurrencyValue;
        protected string CurrencyCode;
        protected int OrderId;
        protected string PageTitle;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            OrderId = Request["orderid"].TryParseInt();

            if (OrderId == 0 || Request["hash"].IsNullOrEmpty() || (_order = OrderService.GetOrder(OrderId)) == null ||
                _order == null || _order.Payed || _order.OrderStatus.IsCanceled)
            {
                Response.Redirect("~/");
                return;
            }

            if (Request["hash"] != OrderService.GetBillingLinkHash(_order))
            {
                Response.Redirect("~/");
                return;
            }

            LoadOrder();
            LoadPayments();

            PageTitle = Resource.Billing_Title + _order.OrderID;
        }

        protected string RenderSelectedOptions(IList<EvaluatedCustomOptions> evlist)
        {
            if (evlist.Count == 0)
                return string.Empty;

            var html = new StringBuilder();
            foreach (EvaluatedCustomOptions ev in evlist)
                html.Append(string.Format("<div>{0}: {1}</div>", ev.CustomOptionTitle, ev.OptionTitle));

            return html.ToString();
        }

        private void LoadOrder()
        {
            CurrencyValue = _order.OrderCurrency.CurrencyValue;
            CurrencyCode = _order.OrderCurrency.CurrencyCode;

            lvOrderList.DataSource = _order.OrderItems;
            lvOrderList.DataBind();


            float prodTotal = 0;
            if (_order.OrderCertificates != null && _order.OrderCertificates.Count > 0)
            {
                prodTotal = _order.OrderCertificates.Sum(item => item.Sum);
            }
            else
            {
                prodTotal = _order.OrderItems.Sum(oi => oi.Price * oi.Amount);
            }

            var totalDiscount = _order.OrderDiscount > 0
                                    ? Convert.ToSingle(Math.Round(prodTotal / 100 * _order.OrderDiscount, 2))
                                    : 0;


            lblProductsPrice.Text = CatalogService.GetStringPrice(prodTotal, CurrencyValue, CurrencyCode);

            if (totalDiscount > 0)
            {
                discountRow.Visible = true;
                liDiscountPercent.Text = string.Format("<span class=\"per\">-{0}%</span>", _order.OrderDiscount);
                lblOrderDiscount.Text = "-" + CatalogService.GetStringPrice(totalDiscount, CurrencyValue, CurrencyCode);
            }
            else
            {
                discountRow.Visible = false;
            }

            if (_order.ShippingCost > 0)
            {
                deliveryRow.Visible = true;
                lblShippingPrice.Text = CatalogService.GetStringPrice(_order.ShippingCost, CurrencyValue, CurrencyCode);
            }


            if (_order.PaymentCost > 0)
            {
                paymentExtraChargeRow.Visible = true;
                lPaymentCost.Text = _order.PaymentCost > 0
                                        ? Resource.Client_OrderConfirmation_PaymentCost
                                        : Resource.Client_OrderConfirmation_PaymentDiscount;
                lblPaymentExtraCharge.Text = (_order.PaymentCost > 0 ? "+" : "") + CatalogService.GetStringPrice(_order.PaymentCost, CurrencyValue, CurrencyCode);
            }

            if (_order.BonusCost > 0)
            {
                bonusesRow.Visible = true;
                lblOrderBonuses.Text = "-" + CatalogService.GetStringPrice(_order.BonusCost, CurrencyValue, CurrencyCode);
            }

            lvTaxes.DataSource = _order.Taxes;
            lvTaxes.DataBind();

            lblTotalPrice.Text = CatalogService.GetStringPrice(_order.Sum, CurrencyValue, CurrencyCode);
        }

        private void LoadPayments()
        {
            List<PaymentMethod> paymentMethods;

            if (_order.OrderCertificates.Any())
            {
                paymentMethods = PaymentService.GetCertificatePaymentMethods();
            }
            else
            {
                var returnPayment = PaymentService.LoadMethods(_order.ShippingMethodId, null, false, false);
                paymentMethods = PaymentService.UseGeoMapping(returnPayment, _order.ShippingContact.Country, _order.ShippingContact.City).ToList();
            }


            if (paymentMethods.All(p => p.PaymentMethodId != _order.PaymentMethodId))
            {
                var payment = PaymentService.GetPaymentMethod(_order.PaymentMethodId);
                if (payment != null)
                {
                    paymentMethods.Add(payment);
                }
            }
            
            var selectedIndex = paymentMethods.FindIndex(item => item.PaymentMethodId == _order.PaymentMethodId);
            lvPaymentMethod.DataSource = paymentMethods;
            lvPaymentMethod.SelectedIndex = selectedIndex != -1 ? selectedIndex : 0;
            lvPaymentMethod.DataBind();
        }
    }
}