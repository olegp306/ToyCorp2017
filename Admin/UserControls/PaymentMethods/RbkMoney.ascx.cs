using System;
using System.Collections.Generic;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Payment;

namespace Admin.UserControls.PaymentMethods
{
    public partial class RbkMoneyControl : ParametersControl
    {
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return _valid || ValidateFormData(new[] { txtEshopId, txtCurrencyValue },new[]{txtCurrencyValue})
                           ? new Dictionary<string, string>
                               {
                                   {RbkmoneyTemplate.EshopId, txtEshopId.Text},
                                   {RbkmoneyTemplate.RecipientCurrency, ddlCurrency.SelectedValue},
                                   {RbkmoneyTemplate.Preference, ddlPaymentSystem.SelectedValue},
                                   {RbkmoneyTemplate.CurrencyValue, txtCurrencyValue.Text},
                               }
                           : null;
            }
            set
            {
                txtEshopId.Text = value.ElementOrDefault(RbkmoneyTemplate.EshopId);
                txtCurrencyValue.Text = value.ElementOrDefault(RbkmoneyTemplate.CurrencyValue);

                ddlCurrency.DataSource = Rbkmoney.Currencies;
                ddlCurrency.DataBind();
                if (ddlCurrency.Items.FindByValue(value.ElementOrDefault(RbkmoneyTemplate.RecipientCurrency)) != null)
                {
                    ddlCurrency.SelectedValue = value.ElementOrDefault(RbkmoneyTemplate.RecipientCurrency);
                }

                ddlPaymentSystem.DataSource = Rbkmoney.PaymentSystems;
                ddlPaymentSystem.DataBind();
                if (ddlPaymentSystem.Items.FindByValue(value.ElementOrDefault(RbkmoneyTemplate.Preference)) != null)
                {
                    ddlPaymentSystem.SelectedValue = value.ElementOrDefault(RbkmoneyTemplate.Preference);
                }
            }
        }
    }
}