using System.Collections.Generic;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Payment;

namespace Admin.UserControls.PaymentMethods
{
    public partial class ZPaymentControl : ParametersControl
    {
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return _valid ||
                       ValidateFormData(new[] {txtPurse, txtPassword, txtSecretKey, txtCurrencyValue},
                                        new[] {txtCurrencyValue})
                           ? new Dictionary<string, string>
                               {
                                   {ZPaymentTemplate.Purse, txtPurse.Text},
                                   {ZPaymentTemplate.Password, txtPassword.Text},
                                   {ZPaymentTemplate.SecretKey, txtSecretKey.Text},
                                   {ZPaymentTemplate.CurrencyValue, txtCurrencyValue.Text},
                               }
                           : null;
            }
            set
            {
                txtPurse.Text = value.ElementOrDefault(ZPaymentTemplate.Purse);
                txtPassword.Text = value.ElementOrDefault(ZPaymentTemplate.Password);
                txtSecretKey.Text = value.ElementOrDefault(ZPaymentTemplate.SecretKey);
                txtCurrencyValue.Text = value.ElementOrDefault(ZPaymentTemplate.CurrencyValue);
            }
        }
   
    }
}