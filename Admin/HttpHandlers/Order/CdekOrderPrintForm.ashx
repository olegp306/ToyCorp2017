<%@ WebHandler Language="C#" Class="Admin.HttpHandlers.Order.CdekOrderPrintForm" %>

using System.Web;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Core.HttpHandlers;

using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Shipping;

namespace Admin.HttpHandlers.Order
{
    public class CdekOrderPrintForm : AdminHandler, IHttpHandler
    {
        public new void ProcessRequest(HttpContext context)
        {
            if (!Authorize(context))
                return;

            var orderId = context.Request["orderId"].TryParseInt();

            var order = OrderService.GetOrder(orderId);
            if (order == null)
            {
                return;
            }

            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            if (shippingMethod.Type != ShippingType.Cdek)
            {
                return;
            }
            
            var fileName = (new Cdek(shippingMethod.Params)).PrintFormOrder(order);
            var fullFilePath = SettingsGeneral.AbsolutePath + "price_temp\\" + fileName;
            if (!System.IO.File.Exists(fullFilePath))
            {
                return;
            }

            CommonHelper.WriteResponseFile(fullFilePath, fileName);
        }
    }
}