//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.itmcompany.ru
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using AdvantShop.Controls;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Taxes;
using AdvantShop.Catalog;

// TODO REWRITE

namespace ClientPages
{
    public partial class Check_BillUa : AdvantShopClientPage
    {
        private BillUa _bill;
        private Order _order;

        protected bool EmptyCheck
        {
            get { return OrderNumber == null || Bill == null; }
        }

        protected string OrderNumber
        {
            get { return Request["ordernumber"]; }
        }

        protected Order Order
        {
            get { return _order ?? (_order = OrderService.GetOrderByNumber(OrderNumber)); }
        }


        protected BillUa Bill
        {
            get
            {
                if (Order == null)
                    return null;

                if (_bill != null)
                    return _bill;

                if (Order.PaymentMethodId == 0)
                    return null;
                PaymentMethod method = PaymentService.GetPaymentMethod(Order.PaymentMethodId);
                if (!(method is BillUa))
                    return null;
                _bill = (BillUa) method;
                return _bill;
            }
        }

        private List<string> Months = new List<string>
            {
                "січня",
                "лютого",
                "березня",
                "квітня",
                "травня",
                "червня",
                "липня",
                "серпня",
                "вересня",
                "жовтня",
                "листопада",
                "грудня"
            };

        protected override void InitializeCulture()
        {
            AdvantShop.Localization.Culture.InitializeCulture();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (EmptyCheck || Order == null)
                return;

            liCompanyName.Text = Bill.CompanyName;
            liCompanyName2.Text = Bill.CompanyName;
            liCompanyCode.Text = Bill.CompanyCode;
            liCredit.Text = Bill.Credit;
            liBankCode.Text = Bill.BankCode;
            liBankName.Text = Bill.BankName;
            liCompanyEssencials.Text = Bill.CompanyEssentials;

            liOrderNum.Text = string.Format("Рахунок на оплату № {0} від {1} {2} {3} р.",
                                            Order.OrderID, Order.OrderDate.Day.ToString("0#"),
                                            Months[Order.OrderDate.Month-1], Order.OrderDate.Year);

            liBuyerInfo.Text = Order.BillingContact.Name;

            rprOrrderItems.DataSource = Order.OrderItems;
            rprOrrderItems.DataBind();

            trDiscount.Visible = Order.TotalDiscount != 0;
            liDiscount.Text = GetPrice(Order.TotalDiscount, Order.OrderCurrency);

            liTotal.Text = GetPrice(Order.Sum, Order.OrderCurrency);
            hfTotal.Value = GetPriceFormat(Order.Sum, Order.OrderCurrency);

            liTotalCount.Text = string.Format("Всього найменувань {0}, на суму {1} грн.",
                                              Order.OrderItems.Count,
                                              GetPrice(Order.Sum, Order.OrderCurrency));

            var taxes = TaxServices.GetOrderTaxes(Order.OrderID).Where(p => p.TaxSum > 0).ToList();
            if (taxes.Count > 0)
            {
                var taxSum = (float) Math.Round(taxes.Sum(t => t.TaxSum), 2);

                liTaxSum.Text = GetPrice(taxSum, Order.OrderCurrency);
                hfTax.Value = GetPriceFormat(taxSum, Order.OrderCurrency);

                trTax.Visible = true;
                trTaxSum.Visible = true;
            }
            else
            {
                trTax.Visible = false;
                trTaxSum.Visible = false;
            }
        }

        protected string GetPrice(float price, OrderCurrency currency)
        {
            return (price/currency.CurrencyValue*Bill.CurrencyValue).ToString("##,##0.00").Replace(",", ".");
        }

        protected string GetPriceFormat(float price, OrderCurrency currency)
        {
            return (price/currency.CurrencyValue*Bill.CurrencyValue).ToString("####0.00").Replace(",", ".");
        }

        protected string GetUnit(int entityId)
        {
            return ProductService.GetProduct(entityId).Unit;
        }
        
    }
}