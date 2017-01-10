<%@ WebHandler Language="C#" Class="GetPayButton" %>


using System.Linq;
using System.Web;
using System.Web.SessionState;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Modules;
using AdvantShop.Orders;
using AdvantShop.Payment;
using Newtonsoft.Json;
using Resources;

public class GetPayButton : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        AdvantShop.Localization.Culture.InitializeCulture();
        context.Response.ContentType = "application/json";

        if (context.Request["orderid"].IsNullOrEmpty() || context.Request["paymentid"].IsNullOrEmpty())
        {
            context.Response.Write(JsonConvert.SerializeObject(string.Empty));
            return;
        }
        
        var order = OrderService.GetOrder(context.Request["orderid"].TryParseInt());
        if (order == null || order.Payed || order.OrderStatus.IsCanceled)
        {
            context.Response.Write(JsonConvert.SerializeObject(string.Empty));
            return;
        }

        var payment = PaymentService.GetPaymentMethod(context.Request["paymentId"].TryParseInt());
        if (payment == null)
        {
            context.Response.Write(JsonConvert.SerializeObject(string.Empty));
            return;
        }

        if (order.PaymentMethodId != payment.PaymentMethodId)
        {
            order.PaymentMethodId = payment.PaymentMethodId;
            order.ArchivedPaymentName = payment.Name;
            order.PaymentCost = payment.ExtrachargeType == ExtrachargeType.Percent
                                    ? payment.Extracharge *
                                      (order.OrderItems.Sum(x => x.Price * x.Amount) - order.TotalDiscount - order.BonusCost +
                                       order.ShippingCost + order.TaxCost) / 100
                                    : payment.Extracharge;
            OrderService.UpdateOrderMain(order);
            OrderService.RefreshTotal(order.OrderID);

            ModulesRenderer.OrderUpdated(order.OrderID);

            order = OrderService.GetOrder(order.OrderID);
        }

        var curValue = order.OrderCurrency.CurrencyValue;
        var curCode = order.OrderCurrency.CurrencyCode;

        var productsPrice = order.OrderItems.Sum(item => item.Amount * item.Price);
        string couponPrice = string.Empty, couponPersent = string.Empty;

        if (order.Coupon != null)
        {
            couponPrice = order.Coupon.Value != 0
                ? string.Format("-{0} ({1})",
                    CatalogService.GetStringPrice(order.TotalDiscount, curValue, curCode),
                    order.Coupon.Code)
                : string.Empty;

            couponPersent = order.Coupon.Type == CouponType.Percent
                ? CatalogService.FormatPriceInvariant(order.Coupon.Value)
                : string.Empty;
        }

        var script = OrderService.ProcessOrder(order, PaymentService.PageWithPaymentButton.orderconfirmation);

        var obj = new
        {
            formString = script.Contains("</form>") ? script.Substring(0, script.IndexOf("</form>") + 7) : "",
            buttonString =
                script.Contains("</form>")
                    ? script.Substring(script.IndexOf("</form>") + 7, script.Length - (script.IndexOf("</form>") + 7))
                    : script,
            TotalDiscountPrice = CatalogService.GetStringPrice(order.TotalDiscount, curValue, curCode),
            ProductsPrice = CatalogService.GetStringPrice(productsPrice, curValue, curCode),
            TotalDiscount = order.OrderDiscount,
            CertificatePrice = order.Certificate != null
                ? CatalogService.GetStringPrice(order.Certificate.Price, curValue, curCode)
                : string.Empty,
            TotalPrice = CatalogService.GetStringPrice(order.Sum, curValue, curCode),
            couponPrice,
            couponPersent,
            ShippingPrice = CatalogService.GetStringPrice(order.ShippingCost, curValue, curCode),
            PaymentPrice = order.PaymentCost != 0
                ? (order.PaymentCost > 0 ? "+" : "") +
                  CatalogService.GetStringPrice(order.PaymentCost, curValue, curCode)
                : string.Empty,
            PaymentPriceText =
                order.PaymentCost != 0
                    ? Resource.Client_OrderConfirmation_PaymentCost
                    : Resource.Client_OrderConfirmation_PaymentDiscount,
            Bonus =
                order.BonusCost > 0 ? CatalogService.GetStringPrice(order.BonusCost, curValue, curCode) : string.Empty,
            TaxesNames = order.Taxes.Select(tax => tax.TaxName).AggregateString(','),
            TaxesPrice = CatalogService.GetStringPrice(order.TaxCost, curValue, curCode),
        };

        context.Response.Write(JsonConvert.SerializeObject(obj));
    }

    public bool IsReusable
    {
        get { return false; }
    }
}
