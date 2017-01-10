//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdvantShop.Catalog;
using AdvantShop.Controls;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;
using Resources;

namespace ClientPages
{
    public partial class Check_Check : AdvantShopClientPage
    {
        private Check _check;
        private Order _order;

        protected bool EmptyCheck
        {
            get { return OrderNumber == null || _Check == null; }
        }

        protected string OrderNumber
        {
            get { return Request["ordernumber"]; }
        }

        protected Order Order
        {
            get { return _order ?? (_order = OrderService.GetOrderByNumber(OrderNumber)); }
        }

        private int MethodID
        {
            get
            {
                int id = 0;
                return int.TryParse(Request["methodid"], out id) ? id : 0;
            }
        }

        protected Check _Check
        {
            get
            {
                if (_check != null)
                    return _check;

                if (MethodID == 0)
                    return null;
                PaymentMethod method = PaymentService.GetPaymentMethod(MethodID);
                if (!(method is Check))
                    return null;
                _check = (Check) method;
                return _check;
            }
        }



        private Currency _currency;

        //protected int OrderId;

        protected override void InitializeCulture()
        {
            AdvantShop.Localization.Culture.InitializeCulture();
        }

        protected string EvalPrice(float price)
        {
            return CatalogService.GetStringPrice(price, _currency);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (EmptyCheck || Order == null)
                return;
            _currency = (Order.OrderCurrency ?? CurrencyService.Currency("USD")) ?? new Currency
                {
                    IsCodeBefore = true,
                    Iso3 = "USD",
                    Name = "USD",
                    PriceFormat = CurrencyService.DefaultPriceFormat,
                    Symbol = "$",
                    Value = 1
                };

            //var check = new Check_PaymentModule();

            lCompanyName.Text = _Check.CompanyName;
            lAddress.Text = _Check.Adress;
            lCountry.Text = _Check.Country;
            lState.Text = _Check.State;
            lCity.Text = _Check.City;

            lCompanyPhone.Text = _Check.Phone;
            lInterPhone.Text = _Check.IntPhone;
            lCompanyFax.Text = _Check.Fax;

            lOrderDate.Text = AdvantShop.Localization.Culture.ConvertDate(Order.OrderDate);
                // AdvantShop.Localization.Culture.ConvertDate((DateTime)reader["OrderDate"]);
            lOrderId.Text = @"#" + Order.OrderID;
            lShippingMethod.Text = Order.ShippingMethodName; // reader["ShippingMethod"].ToString();

            lName.Text = Order.BillingContact.Name;
            lPhone.Text = Order.OrderCustomer.MobilePhone;
            //lFax.Text = Order.BillingContact.Fax;
            //lEmail.Text = Order.BillingContact.Email;
            lEmail.Text = Order.OrderCustomer.Email;

            lBillingAddress.Text = Order.BillingContact.Address;
            lBillingCity.Text = Order.BillingContact.City;
            lBillingState.Text = Order.BillingContact.Zone;
            lBillingCountry.Text = Order.BillingContact.Country;
            lBillingZip.Text = Order.BillingContact.Zip;

            lShippingAddress.Text = Order.ShippingContact.Address;
            lShippingCity.Text = Order.ShippingContact.City;
            lShippingState.Text = Order.ShippingContact.Zone;
            lShippingCountry.Text = Order.ShippingContact.Country;
            lShippingZip.Text = Order.ShippingContact.Zip;



            lSubTotal.Text =
                EvalPrice((Order.Sum - Order.ShippingCost)*100.0F/(100 - Order.OrderDiscount));
            lShippingCost.Text = EvalPrice(Order.ShippingCost);
            lDiscount.Text =
                EvalPrice(Order.OrderDiscount*(Order.Sum - Order.ShippingCost/(100 - Order.OrderDiscount)));


            var shippingContact = new AdvantShop.Customers.CustomerContact
                {
                    CountryId = CountryService.GetCountryIdByName(lShippingCountry.Text),
                    RegionId = RegionService.GetRegionIdByName(lShippingState.Text)
                };
            var billingContact = new AdvantShop.Customers.CustomerContact
                {
                    CountryId = CountryService.GetCountryIdByName(lBillingCountry.Text),
                    RegionId = RegionService.GetRegionIdByName(lBillingState.Text)
                };

            rptOrderItems.DataBind();
            IList<OrderItem> dtOrder = Order.OrderItems;
            var taxedItems = new List<OrderTax>();

            //foreach (OrderItem item in dtOrder)
            //{
            //    if (item.ProductID != null)
            //    {
            //        ICollection<TaxElement> t = TaxServices.GetTaxesForProduct((int) item.ProductID, billingContact,
            //                                                                   shippingContact);
            //        foreach (TaxElement tax in t)
            //        {
            //            TaxValue taxedItem = taxedItems.Find(tv => tv.TaxID == tax.TaxId);
            //            if (taxedItem != null)
            //            {
            //                taxedItem.TaxSum += TaxServices.CalculateTax(item, tax, Order.OrderDiscount);
            //            }
            //            else
            //            {
            //                taxedItems.Add(new TaxValue
            //                    {
            //                        TaxID = tax.TaxId,
            //                        TaxName = tax.Name,
            //                        TaxShowInPrice = tax.ShowInPrice,
            //                        TaxSum = TaxServices.CalculateTax(item, tax, Order.OrderDiscount)
            //                    });
            //            }
            //        }
            //    }
            //}


            literalTaxCost.Text = BuildTaxTable(taxedItems, _currency.Value, _currency.Iso3,
                                                Resource.Admin_ViewOrder_Taxes);
            lTotal.Text = EvalPrice(Order.Sum);


        }

        public static string BuildTaxTable(List<OrderTax> taxes, float currentCurrencyRate, string currentCurrencyIso3,
                                           string message)
        {
            var sb = new StringBuilder();
            if (!taxes.Any())
            {
                sb.Append("<tr><td style=\"padding-right: 3px; height: 20px; width: 100%; text-align: right;\"><strong>");
                sb.Append(message);
                sb.Append(
                    "&nbsp;</strong></td><td style=\"white-space: nowrap; padding-right: 5px; height: 20px; text-align: right;\"><span class=\"currency\">");
                sb.Append(CatalogService.GetStringPrice(0, currentCurrencyRate, currentCurrencyIso3));
                sb.Append("</span></td></tr>");
            }
            else
                foreach (OrderTax tax in taxes)
                {
                    sb.Append(
                        "<tr><td style=\"padding-right: 3px; height: 20px; width: 100%; text-align: right;\"><strong>");
                    sb.Append((tax.TaxShowInPrice ? Resource.Core_TaxServices_Include_Tax : "") + " " + tax.TaxName);
                    sb.Append(
                        ":&nbsp;</strong></td><td style=\"white-space: nowrap; padding-right: 5px; height: 20px; text-align: right;\"><span class=\"currency\">");
                    sb.Append(CatalogService.GetStringPrice(tax.TaxSum, currentCurrencyRate, currentCurrencyIso3));
                    sb.Append("</span></td></tr>");
                }
            return sb.ToString();
        }
    }
}