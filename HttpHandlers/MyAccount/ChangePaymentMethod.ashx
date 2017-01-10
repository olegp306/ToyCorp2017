<%@ WebHandler Language="C#" Class="ChangePaymentMethod" %>

using System.Linq;
using System.Web;
using System.Web.SessionState;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Payment;
using Newtonsoft.Json;
using AdvantShop;

public class ChangePaymentMethod : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        if (!AdvantShop.Customers.CustomerContext.CurrentCustomer.RegistredUser)
        {
            context.Response.Write(JsonConvert.SerializeObject(string.Empty));
            return;
        }

        if (context.Request["paymentId"].IsNullOrEmpty() && context.Request["paymentName"].IsNullOrEmpty() &&
            SQLDataHelper.GetString(context.Request["orderNumber"]).IsNullOrEmpty())
        {
            context.Response.Write(JsonConvert.SerializeObject(string.Empty));
            return;
        }

        var order = OrderService.GetOrderByNumber(SQLDataHelper.GetString(context.Request["orderNumber"]));
        if (order == null)
        {
            context.Response.Write(JsonConvert.SerializeObject(string.Empty));
            return;
        }
        context.Response.ContentType = "application/json";
        var payment = PaymentService.GetPaymentMethod(System.Convert.ToInt32(context.Request["paymentId"]));


        if (payment == null)
        {
            context.Response.Write(JsonConvert.SerializeObject(string.Empty));
            return;
        }
        order.PaymentMethodId = payment.PaymentMethodId;
        order.ArchivedPaymentName = payment.Name;
        order.PaymentCost = payment.ExtrachargeType == ExtrachargeType.Percent
                                ? payment.Extracharge*
                                  (order.OrderItems.Sum(x => x.Price*x.Amount) - order.TotalDiscount - order.BonusCost +
                                   order.ShippingCost + order.TaxCost)/100
                                : payment.Extracharge;
        OrderService.UpdateOrderMain(order);
        OrderService.RefreshTotal(order.OrderID);
        
        AdvantShop.Modules.ModulesRenderer.OrderUpdated(order.OrderID);
    }

    public bool IsReusable
    {
        get { return false; }
    }
}
