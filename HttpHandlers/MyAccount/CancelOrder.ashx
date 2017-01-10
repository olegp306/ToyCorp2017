<%@ WebHandler Language="C#" Class="HttpHandlers.MyAccount.ChangeOrderStatus" %>
using System.Linq;
using System.Web;
using System.Web.SessionState;
using AdvantShop.Configuration;
using AdvantShop.Mails;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using Newtonsoft.Json;
using AdvantShop;

namespace HttpHandlers.MyAccount
{
    public class ChangeOrderStatus : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            if (!AdvantShop.Customers.CustomerContext.CurrentCustomer.RegistredUser ||
                context.Request["ordernumber"].IsNullOrEmpty())
            {
                context.Response.Write(JsonConvert.SerializeObject(string.Empty));
                return;
            }

            var order = OrderService.GetOrderByNumber(context.Request["ordernumber"]);
            var customer = OrderService.GetOrderCustomer(context.Request["ordernumber"]);

            if (order == null || customer == null ||
                customer.CustomerID != AdvantShop.Customers.CustomerContext.CurrentCustomer.Id)
            {
                context.Response.Write(JsonConvert.SerializeObject(string.Empty));
                return;
            }

            OrderService.CancelOrder(order.OrderID);

            // geting new actual data for email
            
            order = OrderService.GetOrder(order.OrderID);

            var productPrice = order.OrderItems.Sum(item => item.Price * item.Amount);
            var orderTable = OrderService.GenerateHtmlOrderTable(order.OrderItems,
                                                                  CurrencyService.CurrentCurrency, productPrice,
                                                                  order.OrderDiscount, order.Coupon,
                                                                  order.Certificate,
                                                                  order.OrderDiscount > 0
                                                                      ? order.OrderDiscount * productPrice / 100
                                                                      : 0, order.ShippingCost, order.PaymentCost, order.TaxCost, order.BonusCost, 0);
            
            var mailTemplate = new OrderStatusMailTemplate(order.OrderID.ToString(), order.OrderStatus.StatusName,
                                                                     order.StatusComment.Replace("\r\n", "<br />"),
                                                                     order.Number, orderTable);
            mailTemplate.BuildMail();
            SendMail.SendMailNow(SettingsMail.EmailForOrders, mailTemplate.Subject, mailTemplate.Body, true);
            SendMail.SendMailNow(order.OrderCustomer.Email, mailTemplate.Subject, mailTemplate.Body, true);
            
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }

}
