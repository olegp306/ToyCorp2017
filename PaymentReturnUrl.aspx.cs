//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Payment;
using Resources;

namespace ClientPages
{
    public partial class PaymentReturnUrl : AdvantShopClientPage
    {
        protected int PaymentMethodID
        {
            get
            {
                int id;
                return int.TryParse(Request["paymentmethodid"], out id) ? id : 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PaymentMethodID < 1)
                return;

            var method = PaymentService.GetPaymentMethod(PaymentMethodID);
            if (method != null && (method.NotificationType & NotificationType.ReturnUrl) == NotificationType.ReturnUrl)
            {
                var res = method.ProcessResponse(Context);
                if (res.IsNotEmpty())
                {
                    lblResult.Text = Resource.Client_PaymentReturnUrl_Status + res;
                }
                else
                {
                    lblResult.Visible = false;
                }
            }
        }
    }
}