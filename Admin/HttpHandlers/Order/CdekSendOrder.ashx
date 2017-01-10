<%@ WebHandler Language="C#" Class="Admin.HttpHandlers.Order.CdekSendOrder" %>

using System;
using System.Web;
using AdvantShop;
using AdvantShop.Core.HttpHandlers;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Shipping.ShippingCdek;
using Newtonsoft.Json;

namespace Admin.HttpHandlers.Order
{
    public class CdekSendOrder : AdminHandler, IHttpHandler
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
            if (shippingMethod.Type != ShippingType.Cdek)
            {
                ReturnResult(context, new { status = false, message = "error" });
                return;
            }

            var tariffId = order.OrderPickPoint.AdditionalData.TryParseInt();
            var pickpointId = order.OrderPickPoint.PickPointId;
            

            CdekStatusAnswer result = (new Cdek(shippingMethod.Params)).SendNewOrders(order, tariffId, pickpointId);

            ReturnResult(context, new { status = result.Status, message = result.Message });
        }

        private static void ReturnResult(HttpContext context, object result)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(new { result }));
            context.Response.End();
        }
    }
}