<%@ WebHandler Language="C#" Class="Admin.HttpHandlers.Order.SendMailOrderStatus" %>

using System;
using System.Linq;
using System.Web;
using AdvantShop.Core.HttpHandlers;
using AdvantShop.Mails;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using Resources;

namespace Admin.HttpHandlers.Order
{
    public class SendMailOrderStatus : AdminHandler, IHttpHandler
    {
        public new void ProcessRequest(HttpContext context)
        {
            if (!Authorize(context))
                return;

            var orderId = 0;

            if (!Int32.TryParse(context.Request["orderid"], out orderId))
            {
                ReturnResult(context, "error");
            }

            var order = OrderService.GetOrder(orderId);
            if (order != null)
            {

                var productPrice = order.OrderItems.Sum(item => item.Price * item.Amount);
                var orderTable = OrderService.GenerateHtmlOrderTable(order.OrderItems,
                                                                      CurrencyService.CurrentCurrency, productPrice,
                                                                      order.OrderDiscount, order.Coupon,
                                                                      order.Certificate,
                                                                      order.OrderDiscount > 0
                                                                          ? order.OrderDiscount * productPrice / 100
                                                                          : 0, order.ShippingCost, order.PaymentCost, order.TaxCost, order.BonusCost, 0);


                var mailTemplate = new OrderStatusMailTemplate(orderId.ToString(), order.OrderStatus.StatusName,
                                                                     order.StatusComment.Replace("\r\n", "<br />"),
                                                                     order.Number, orderTable);
                mailTemplate.BuildMail();

                SendMail.SendMailNow(order.OrderCustomer.Email, mailTemplate.Subject, mailTemplate.Body, true);
                ReturnResult(context, string.Empty);
            }

            ReturnResult(context, "error");
        }

        private static void ReturnResult(HttpContext context, string result)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { result }));
            context.Response.End();
        }
    }
}