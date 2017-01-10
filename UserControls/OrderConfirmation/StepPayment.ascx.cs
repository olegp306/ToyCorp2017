using System;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Helpers;

namespace UserControls.OrderConfirmation
{
    public partial class StepPayment : System.Web.UI.UserControl
    {
        public OrderConfirmationData PageData { get; set; }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (PageData == null) 
                return;
            
            LoadPayment();

            switch (PageData.SelectedPaymentItem.Type)
            {
                case PaymentType.SberBank:
                    pnlInfoForSberBank.Attributes["style"] = "display: block;";
                    break;
                case PaymentType.Bill:
                    pnlInfoForBill.Attributes["style"] = "display: block;";
                    break;
                case PaymentType.QIWI:
                    pnlPhoneForQiwi.Attributes["style"] = "display: block;";
                    txtPhone.Text = PageData.Customer.Phone;
                    break;
            }
        }

        private void LoadPayment()
        {
            if (PageData.BillingContact != null)
            {
                var shpCart = ShoppingCartService.CurrentShoppingCart;
                var showCertificate = SettingsOrderConfirmation.EnableGiftCertificateService &&
                                      shpCart.Certificate != null &&
                                      shpCart.TotalPrice - shpCart.TotalDiscount + PageData.SelectedShippingItem.Rate + PageData.TaxesTotal <= 0;
                
                var paymentId = CommonHelper.GetCookieString("payment").TryParseInt();
                if (paymentId != 0)
                {
                    pm.SelectedId = paymentId;
                    CommonHelper.DeleteCookie("payment");
                }
                else
                {
                    pm.SelectedId = PageData.SelectedPaymentItem.PaymenMethodtId;
                }

                pm.LoadMethods(PageData.BillingContact.Country, PageData.BillingContact.City,
                    PageData.SelectedShippingItem.MethodId, PageData.SelectedShippingItem.Ext, showCertificate, false);

                PageData.SelectedPaymentItem = pm.SelectedItem;
            }
        }

        public void UpdatePageData(OrderConfirmationData orderConfirmationData)
        {
            switch (orderConfirmationData.SelectedPaymentItem.Type)
            {
                case PaymentType.Bill:
                    orderConfirmationData.PaymentDetails = new PaymentDetails { CompanyName = txtCompanyName.Text, INN = txtINN.Text };
                    break;
                case PaymentType.SberBank:
                    orderConfirmationData.PaymentDetails = new PaymentDetails { CompanyName = String.Empty, INN = txtINN2.Text };
                    break;
                case PaymentType.QIWI:
                    orderConfirmationData.PaymentDetails = new PaymentDetails { Phone = txtPhone.Text };
                    break;
                default:
                    orderConfirmationData.PaymentDetails = null;
                    break;
            }
        }
    }
}