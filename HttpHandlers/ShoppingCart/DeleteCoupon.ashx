<%@ WebHandler Language="C#" Class="DeleteCoupon" %>

using System.Web;
using System.Web.SessionState;
using AdvantShop.Catalog;

public class DeleteCoupon : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        var coupon =  CouponService.GetCustomerCoupon();
        if(coupon != null)
        {
            CouponService.DeleteCustomerCoupon(coupon.CouponID);
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}
