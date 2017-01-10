using System;
using System.Collections.Generic;
using System.IO;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Helpers;
using AdvantShop.Payment;

namespace Admin.UserControls.PaymentMethods
{
    public partial class YandexKassaControl : ParametersControl
    {
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return _valid ||
                       ValidateFormData(
                           new[] { txtShopID, txtScID, txtCurrencyValue, txtPassword }, //, txtCertificate
                           new[] {txtCurrencyValue}, new[] {txtShopID})
                           ? new Dictionary<string, string>
                               {
                                   {YandexKassaTemplate.ShopID, txtShopID.Text},
                                   {YandexKassaTemplate.ScID, txtScID.Text},
                                   {YandexKassaTemplate.CurrencyValue, txtCurrencyValue.Text},
                                   {YandexKassaTemplate.YaPaymentType, ddlPaymentType.SelectedValue ?? ddlPaymentType.Items[0].Value},
                                   {YandexKassaTemplate.DemoMode, cbDemoMode.Checked.ToString()},
                                   {YandexKassaTemplate.Password, txtPassword.Text},
                               }
                           : null;
            }
            set
            {
                txtShopID.Text = value.ElementOrDefault(YandexKassaTemplate.ShopID);
                txtScID.Text = value.ElementOrDefault(YandexKassaTemplate.ScID);
                txtCurrencyValue.Text = value.ElementOrDefault(YandexKassaTemplate.CurrencyValue);
                cbDemoMode.Checked = value.ElementOrDefault(YandexKassaTemplate.DemoMode).TryParseBool();
                txtPassword.Text = value.ElementOrDefault(YandexKassaTemplate.Password);
                if (ddlPaymentType.Items.FindByValue(value.ElementOrDefault(YandexKassaTemplate.YaPaymentType)) != null)
                {
                    ddlPaymentType.SelectedValue = value.ElementOrDefault(YandexKassaTemplate.YaPaymentType);
                }
            }
        }
    }
}