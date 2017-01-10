<%@ WebHandler Language="C#" Class="Admin.HttpHandlers.Order.CheckoutSendOrder" %>

using System.Web;
using AdvantShop;
using AdvantShop.Core.HttpHandlers;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using Newtonsoft.Json;

namespace Admin.HttpHandlers.Order
{
    public class CheckoutSendOrder : AdminHandler, IHttpHandler
    {
        public new void ProcessRequest(HttpContext context)
        {
            if (!Authorize(context))
            {
                return;
            }

            var orderId = context.Request["orderId"].TryParseInt();
            
            if (orderId == 0)
            {
                ReturnResult(context, new { status = false, message = "error" });
                return;
            }

            var order = OrderService.GetOrder(orderId);
            if (order == null || order.OrderPickPoint == null)
            {
                ReturnResult(context, new { status = false, message = "error" });
                return;
            }

            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            if (shippingMethod.Type != ShippingType.CheckoutRu)
            {
                ReturnResult(context, new { status = false, message = "error" });
                return;
            }
            
            //var result = 
            var response = (new ShippingCheckoutRu(shippingMethod.Params)).CreateOrder(order);

            ReturnResult(context, new { error = response.error, message = response.error ? "Ошибка при добавлении заказа, ответ сервера : " + response.errorMessage : "Заказ добавлен в систему под номером " + response.order.id});
        }

        private static void ReturnResult(HttpContext context, object result)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(new { result }));
            context.Response.End();
        }
    }
}