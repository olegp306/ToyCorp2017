<%@ WebHandler Language="C#" Class="PaymentNotification" %>

using System.Web;
using System.Web.SessionState;
using AdvantShop;
using AdvantShop.Payment;

public class PaymentNotification : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        var method = PaymentService.GetPaymentMethod(GetMethodId(context));
        if (method != null && (method.NotificationType & NotificationType.Handler) == NotificationType.Handler)
        {
            var paymentResponse =  method.ProcessResponse(context);
            if (paymentResponse.IsNotEmpty())
                context.Response.Write(paymentResponse);
        }
    }

    private static int GetMethodId(HttpContext context)
    {
        int id;
        var stringId = context.Request["PaymentMethodID"];
        return !string.IsNullOrEmpty(stringId) && int.TryParse(stringId, out id) ? id : 0;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}