using System.Collections.Generic;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Payment;

namespace Admin.UserControls.PaymentMethods
{
    public partial class BillControl : ParametersControl
    {
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return _valid || ValidateFormData(new[]
                    {
                        txtCurrencyValue,
                        txtCompanyName,
                        txtTransAccount,
                        txtCorAccount,
                        txtAddress,
                        txtTelephone,
                        txtINN,
                        txtBIK,
                        txtBankName,
                        txtDirector,
                        txtAccountant,
                    },
                                                  new[] { txtCurrencyValue },
                                                  null)
                           ? new Dictionary<string, string>
                               {
                                   {BillTemplate.Accountant, txtAccountant.Text},
                                   {BillTemplate.CurrencyValue, txtCurrencyValue.Text},
                                   {BillTemplate.CompanyName, txtCompanyName.Text},
                                   {BillTemplate.TransAccount, txtTransAccount.Text},
                                   {BillTemplate.CorAccount, txtCorAccount.Text},
                                   {BillTemplate.Address, txtAddress.Text},
                                   {BillTemplate.Telephone, txtTelephone.Text},
                                   {BillTemplate.INN, txtINN.Text},
                                   {BillTemplate.KPP, txtKPP.Text},
                                   {BillTemplate.BIK, txtBIK.Text},
                                   {BillTemplate.BankName, txtBankName.Text},
                                   {BillTemplate.Director, txtDirector.Text},
                                   {BillTemplate.Manager, txtManager.Text}
                               }
                           : null;
            }
            set
            {
                txtAccountant.Text = value.ElementOrDefault(BillTemplate.Accountant).IsNotEmpty() ? value.ElementOrDefault(BillTemplate.Accountant) : SettingsBank.Accountant;
                txtCompanyName.Text = value.ElementOrDefault(BillTemplate.CompanyName).IsNotEmpty() ? value.ElementOrDefault(BillTemplate.CompanyName) : SettingsBank.CompanyName;
                txtTransAccount.Text = value.ElementOrDefault(BillTemplate.TransAccount).IsNotEmpty() ? value.ElementOrDefault(BillTemplate.TransAccount) : SettingsBank.RS;
                txtCorAccount.Text = value.ElementOrDefault(BillTemplate.CorAccount).IsNotEmpty() ? value.ElementOrDefault(BillTemplate.CorAccount) : SettingsBank.KS;
                txtAddress.Text = value.ElementOrDefault(BillTemplate.Address).IsNotEmpty() ? value.ElementOrDefault(BillTemplate.Address) : SettingsBank.Address;
                txtTelephone.Text = value.ElementOrDefault(BillTemplate.Telephone).IsNotEmpty() ? value.ElementOrDefault(BillTemplate.Telephone) : SettingsMain.Phone;
                txtINN.Text = value.ElementOrDefault(BillTemplate.INN).IsNotEmpty() ? value.ElementOrDefault(BillTemplate.INN) : SettingsBank.INN;
                txtKPP.Text = value.ElementOrDefault(BillTemplate.KPP);
                txtBIK.Text = value.ElementOrDefault(BillTemplate.BIK).IsNotEmpty() ? value.ElementOrDefault(BillTemplate.BIK) : SettingsBank.BIK;
                txtBankName.Text = value.ElementOrDefault(BillTemplate.BankName).IsNotEmpty() ? value.ElementOrDefault(BillTemplate.BankName) : SettingsBank.BankName;
                txtDirector.Text = value.ElementOrDefault(BillTemplate.Director).IsNotEmpty() ? value.ElementOrDefault(BillTemplate.Director) : SettingsBank.Director;
                txtManager.Text = value.ElementOrDefault(BillTemplate.Manager);
                txtCurrencyValue.Text = value.ElementOrDefault(BillTemplate.CurrencyValue);
            }
        }
    }
}