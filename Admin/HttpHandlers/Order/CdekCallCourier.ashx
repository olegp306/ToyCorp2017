<%@ WebHandler Language="C#" Class="Admin.HttpHandlers.Order.CdekCallCourier" %>

using System.Web;
using AdvantShop;
using AdvantShop.Core.HttpHandlers;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Shipping.ShippingCdek;
using Newtonsoft.Json;

namespace Admin.HttpHandlers.Order
{
    public class CdekCallCourier : AdminHandler, IHttpHandler
    {
        public new void ProcessRequest(HttpContext context)
        {
            if (!Authorize(context))
                return;

            var orderId = context.Request["orderId"].TryParseInt();
          
            var order = OrderService.GetOrder(orderId);
            if (order == null)
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

            //CdekStatusAnswer result = (new Cdek(shippingMethod.Params)).CallCourier(order);

            //ReturnResult(context, new { status = result.Status, message = result.Message });
        }

        private static void ReturnResult(HttpContext context, object result)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(new { result }));
            context.Response.End();
        }
    }
}