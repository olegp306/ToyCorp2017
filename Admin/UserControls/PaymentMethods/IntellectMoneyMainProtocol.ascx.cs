using System;
using System.Collections.Generic;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Payment;

namespace Admin.UserControls.PaymentMethods
{
    public partial class IntellectMoneyMainProtocolControl : ParametersControl
    {
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return _valid || ValidateFormData(new[] { txtEshopId, txtCurrencyValue },new[]{txtCurrencyValue})
                           ? new Dictionary<string, string>
                               {
                                   {IntellectMoneyMainProtocolTemplate.EshopId, txtEshopId.Text},
                                   {IntellectMoneyMainProtocolTemplate.RecipientCurrency, ddlCurrency.SelectedValue},
                                   {IntellectMoneyMainProtocolTemplate.Preference, ddlPaymentSystem.SelectedValue},
                                   {IntellectMoneyMainProtocolTemplate.CurrencyValue, txtCurrencyValue.Text},
                               }
                           : null;
            }
            set
            {
                txtEshopId.Text = value.ElementOrDefault(IntellectMoneyMainProtocolTemplate.EshopId);
                txtCurrencyValue.Text = value.ElementOrDefault(IntellectMoneyMainProtocolTemplate.CurrencyValue);

                ddlCurrency.DataSource = IntellectMoneyMainProtocol.Currencies;
                ddlCurrency.DataBind();
                if (ddlCurrency.Items.FindByValue(value.ElementOrDefault(IntellectMoneyMainProtocolTemplate.RecipientCurrency)) != null)
                {
                    ddlCurrency.SelectedValue = value.ElementOrDefault(IntellectMoneyMainProtocolTemplate.RecipientCurrency);
                }

                ddlPaymentSystem.DataSource = IntellectMoneyMainProtocol.PaymentSystems;
                ddlPaymentSystem.DataBind();
                if (ddlPaymentSystem.Items.FindByValue(value.ElementOrDefault(IntellectMoneyMainProtocolTemplate.Preference)) != null)
                {
                    ddlPaymentSystem.SelectedValue = value.ElementOrDefault(IntellectMoneyMainProtocolTemplate.Preference);
                }
            }
        }
    }
}