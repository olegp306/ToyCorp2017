<%@ WebHandler Language="C#" Class="Admin.HttpHandlers.Order.CreateOrderMultiship" %>

using System;
using System.Web;
using AdvantShop;
using AdvantShop.Core.HttpHandlers;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using Newtonsoft.Json;

namespace Admin.HttpHandlers.Order
{
    public class CreateOrderMultiship : AdminHandler, IHttpHandler
    {
        public new void ProcessRequest(HttpContext context)
        {
            if (!Authorize(context))
                return;

            var orderId = context.Request["orderId"].TryParseInt();
            var order = OrderService.GetOrder(orderId);

            if (order == null)
            {
                ReturnResult(context, "error");
                return;
            }

            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            if (shippingMethod.Type != ShippingType.Multiship)
            {
                ReturnResult(context, "error");
                return;
            }

            var result = false;
            try
            {
                result = MultishipService.CreateOrder(order);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                ReturnResult(context, "error");
            }

            ReturnResult(context, result ? "success" : "error");
        }

        private static void ReturnResult(HttpContext context, string result)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(new {result}));
            context.Response.End();
        }
    }
}