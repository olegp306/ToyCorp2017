using System.Collections.Generic;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Payment;

namespace Admin.UserControls.PaymentMethods
{
    public partial class KupivkreditControl : ParametersControl
    {
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return _valid ||
                       ValidateFormData(new[] { txtPartnerId, txtSecretKey, txtMinimumPrice, txtFirstPayment },
                                        new[] { txtMinimumPrice, txtFirstPayment })
                           ? new Dictionary<string, string>
                               {
                                   {KupivkreditTemplate.PartnerId, txtPartnerId.Text},
                                   {KupivkreditTemplate.SecretKey, txtSecretKey.Text},
                                   {KupivkreditTemplate.Sandbox, chkSandbox.Checked.ToString()},
                                   {KupivkreditTemplate.MinimumPrice, txtMinimumPrice.Text},
                                   {KupivkreditTemplate.FirstPayment, txtFirstPayment.Text}
                               }
                           : null;
            }
            set
            {
                txtPartnerId.Text = value.ElementOrDefault(KupivkreditTemplate.PartnerId);
                txtSecretKey.Text = value.ElementOrDefault(KupivkreditTemplate.SecretKey);
                chkSandbox.Checked = value.ElementOrDefault(KupivkreditTemplate.Sandbox).TryParseBool();
                txtMinimumPrice.Text = value.ElementOrDefault(KupivkreditTemplate.MinimumPrice);
                txtFirstPayment.Text = value.ElementOrDefault(KupivkreditTemplate.FirstPayment);
            }
        }

    }
}