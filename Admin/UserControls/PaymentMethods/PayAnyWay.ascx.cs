using System.Collections.Generic;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Payment;

namespace Admin.UserControls.PaymentMethods
{
    public partial class PayAnyWayControl : ParametersControl
    {
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return _valid || ValidateFormData(new[] { txtMerchantId, txtCurrency, txtSecretKey, txtCurrencyValue }, new[] { txtCurrencyValue })
                           ? new Dictionary<string, string>
                               {

                                   {PayAnyWayTemplate.MerchantId, txtMerchantId.Text},
                                   {PayAnyWayTemplate.CurrencyLabel, txtCurrency.Text},
                                   {PayAnyWayTemplate.Signature, txtSecretKey.Text},
                                   {PayAnyWayTemplate.CurrencyValue, txtCurrencyValue.Text},
                                   {PayAnyWayTemplate.TestMode, chkSandbox.Checked.ToString()},
                                   {PayAnyWayTemplate.UnitId, txtUnitId.Text.Trim()},
                                   {PayAnyWayTemplate.LimitIds, txtLimitIds.Text.Trim()},
                               }
                           : null;
            }
            set
            {
                txtMerchantId.Text = value.ElementOrDefault(PayAnyWayTemplate.MerchantId);
                txtCurrency.Text = value.ElementOrDefault(PayAnyWayTemplate.CurrencyLabel);
                txtSecretKey.Text = value.ElementOrDefault(PayAnyWayTemplate.Signature);
                txtCurrencyValue.Text = value.ElementOrDefault(PayAnyWayTemplate.CurrencyValue);
                txtUnitId.Text = value.ElementOrDefault(PayAnyWayTemplate.UnitId);
                txtLimitIds.Text = value.ElementOrDefault(PayAnyWayTemplate.LimitIds);
                
                bool boolval;
                chkSandbox.Checked = !bool.TryParse(value.ElementOrDefault(PayAnyWayTemplate.TestMode), out boolval) || boolval;
            }
        }
    }
}