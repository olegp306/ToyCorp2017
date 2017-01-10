using System.Collections.Generic;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Payment;

namespace Admin.UserControls.PaymentMethods
{
    public partial class BillUaControl : ParametersControl
    {

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return _valid ||
                       ValidateFormData(new[] { txtCurrencyValue, txtCompanyName, txtCompanyCode, txtBankName, txtBankCode, txtCredit, txtCompanyEssentials }, null, null)
                           ? new Dictionary<string, string>
                               {
                                   {BillUaTemplate.CurrencyValue, txtCurrencyValue.Text},
                                   {BillUaTemplate.CompanyName, txtCompanyName.Text},
                                   {BillUaTemplate.CompanyCode, txtCompanyCode.Text},
                                   {BillUaTemplate.BankName, txtBankName.Text},
                                   {BillUaTemplate.BankCode, txtBankCode.Text},
                                   {BillUaTemplate.Credit, txtCredit.Text},
                                   {BillUaTemplate.CompanyEssentials, txtCompanyEssentials.Text}
                               }
                           : null;
            }
            set
            {
                txtCurrencyValue.Text = value.ElementOrDefault(BillUaTemplate.CurrencyValue);
                txtCompanyName.Text = value.ElementOrDefault(BillUaTemplate.CompanyName);
                txtCompanyCode.Text = value.ElementOrDefault(BillUaTemplate.CompanyCode);
                txtBankName.Text = value.ElementOrDefault(BillUaTemplate.BankName);
                txtBankCode.Text = value.ElementOrDefault(BillUaTemplate.BankCode);
                txtCredit.Text = value.ElementOrDefault(BillUaTemplate.Credit);
                txtCompanyEssentials.Text = value.ElementOrDefault(BillUaTemplate.CompanyEssentials);
            }
        }

    }
}