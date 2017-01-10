<%@ WebHandler Language="C#" Class="Admin.HttpHandlers.Order.SendBillingLink" %>

using System.Linq;
using System.Text;
using System.Web;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Core.HttpHandlers;
using AdvantShop.Customers;
using AdvantShop.Mails;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using Resources;

namespace Admin.HttpHandlers.Order
{
    public class SendBillingLink : AdminHandler, IHttpHandler
    {
        public new void ProcessRequest(HttpContext context)
        {
            if (!Authorize(context))
                return;

            var order = OrderService.GetOrder(context.Request["orderid"].TryParseInt());
            if (order == null)
            {
                ReturnResult(context, "error");
                return;   
            }

            var currency = (Currency) order.OrderCurrency;
            var shopCurrency = CurrencyService.GetCurrencyByIso3(currency.Iso3);
            currency.PriceFormat = shopCurrency != null ? shopCurrency.PriceFormat: string.Empty;
            
            var orderTable = OrderService.GenerateHtmlOrderTable(order.OrderItems, currency, order.OrderItems.Sum(x => x.Price * x.Amount), 
                                                                 order.OrderDiscount, order.Coupon,
                                                                 order.Certificate, order.TotalDiscount, order.ShippingCost,
                                                                 order.PaymentCost, order.TaxCost, order.BonusCost, 0);

            var mailTemplate = new BillingLinkMailTemplate(order.OrderID.ToString(), order.OrderCustomer.FirstName, 
                                                            BuildCustomerContacts(order.OrderCustomer, order.ShippingContact),
                                                            OrderService.GetBillingLinkHash(order), "", orderTable);

            mailTemplate.BuildMail();
            SendMail.SendMailNow(order.OrderCustomer.Email, mailTemplate.Subject, mailTemplate.Body, true);
            SendMail.SendMailNow(SettingsMail.EmailForOrders, mailTemplate.Subject, mailTemplate.Body, true);

            ReturnResult(context, "ok");
        }

        private static void ReturnResult(HttpContext context, string result)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { result }));
            context.Response.End();
        }

        private static string BuildCustomerContacts(OrderCustomer customer, OrderContact contact)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(customer.FirstName))
                sb.AppendFormat(Resource.Client_Registration_Name + " {0}<br/>", customer.FirstName);

            if (!string.IsNullOrEmpty(customer.LastName))
                sb.AppendFormat(Resource.Client_Registration_Surname + " {0}<br/>", customer.LastName);

            if (!string.IsNullOrEmpty(contact.Country))
                sb.AppendFormat(Resource.Client_Registration_Country + " {0}<br/>", contact.Country);

            if (!string.IsNullOrEmpty(contact.Zone))
                sb.AppendFormat(Resource.Client_Registration_State + " {0}<br/>", contact.Zone);

            if (!string.IsNullOrEmpty(contact.City))
                sb.AppendFormat(Resource.Client_Registration_City + " {0}<br/>", contact.City);

            if (!string.IsNullOrEmpty(contact.Zip))
                sb.AppendFormat(Resource.Client_Registration_Zip + " {0}<br/>", contact.Zip);

            if (!string.IsNullOrEmpty(contact.Address))
                sb.AppendFormat(Resource.Client_Registration_Address + ": {0}<br/>", string.IsNullOrEmpty(contact.Address)
                                                                                              ? Resource.Client_OrderConfirmation_NotDefined
                                                                                              : contact.Address);
            return sb.ToString();
        }
    }
}