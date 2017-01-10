using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI;
using AdvantShop;
using AdvantShop.Helpers;
using AdvantShop.Payment;
using AdvantShop.Shipping;

namespace UserControls.OrderConfirmation
{
    public partial class PaymentMethods : UserControl
    {
        #region Fields
        
        private List<PaymentItem> _allPaymentMethods;
        
        public int SelectedId
        {
            get { return hfPaymentMethodId.Value.TryParseInt(); }
            set
            {
                if (_allPaymentMethods.Find(item => item.PaymenMethodtId == value) != null)
                    hfPaymentMethodId.Value = value.ToString();
            }
        }

        private PaymentItem _selectedItem;
        public PaymentItem SelectedItem
        {
            get
            {
                _selectedItem = _allPaymentMethods.Find(item => item.PaymenMethodtId == SelectedId);
                return _selectedItem ?? (_selectedItem = new PaymentItem());
            }
        }

        #endregion

        protected void Page_Init(object sender, EventArgs e)
        {
            _allPaymentMethods = PaymentService.GetAllPaymentMethods(true).Select(item => (PaymentItem) item).ToList();
        }

        public void LoadMethods(string country, string city, int shippingMethodId,
            ShippingOptionEx shippingOptionExt, bool displayCertificateMetod, bool hideCashMetod)
        {
            var returnPayment = PaymentService.LoadMethods(shippingMethodId, shippingOptionExt, displayCertificateMetod,
                hideCashMetod);
            var paymentMethods = PaymentService.UseGeoMapping(returnPayment, country, city);

            var selectedIndex = paymentMethods.FindIndex(item => item.PaymentMethodId == SelectedId);
            lvPaymentMethod.DataSource = paymentMethods;
            lvPaymentMethod.SelectedIndex = selectedIndex != -1 ? selectedIndex : selectedIndex = 0;
            lvPaymentMethod.DataBind();

            if (paymentMethods.Count > 0)
            {
                hfPaymentMethodId.Value = paymentMethods[selectedIndex].PaymentMethodId.ToString();
            }
        }
    }
}