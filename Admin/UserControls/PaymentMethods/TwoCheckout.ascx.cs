using System.Collections.Generic;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Payment;

namespace Admin.UserControls.PaymentMethods
{
    public partial class TwoCheckoutControl : ParametersControl
    {
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return _valid || ValidateFormData(new[] {txtSid, txtSecretWord, txtCurrencyValue}, new[] {txtCurrencyValue})
                           ? new Dictionary<string, string>
                               {
                                   {TwoCheckoutTemplate.Sid, txtSid.Text},
                                   {TwoCheckoutTemplate.Sandbox, chkSandbox.Checked.ToString()},
                                   {TwoCheckoutTemplate.SecretWord, txtSecretWord.Text},
                                   {TwoCheckoutTemplate.CurrencyValue, txtCurrencyValue.Text}
                               }
                           : null;
            }
            set
            {
                txtSid.Text = value.ElementOrDefault(TwoCheckoutTemplate.Sid);
                chkSandbox.Checked = value.ElementOrDefault(TwoCheckoutTemplate.Sandbox).TryParseBool();
                txtSecretWord.Text = value.ElementOrDefault(TwoCheckoutTemplate.SecretWord);
                txtCurrencyValue.Text = value.ElementOrDefault(TwoCheckoutTemplate.CurrencyValue);
            }
        }
    
    }
}