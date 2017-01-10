using System.Collections.Generic;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Payment;

namespace Admin.UserControls.PaymentMethods
{
    public partial class YandexMoneyControl : ParametersControl
    {
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return _valid ||
                    ValidateFormData(new[] {txtShopID, txtScID, txtCurrencyValue},new[] {txtCurrencyValue}, new[] {txtShopID})
                    ? new Dictionary<string, string>
                    {
                        {YandexMoneyTemplate.ShopId, txtShopID.Text},
                        {YandexMoneyTemplate.ScId, txtScID.Text},
                        {YandexMoneyTemplate.CurrencyValue, txtCurrencyValue.Text},
                        {YandexMoneyTemplate.YaPaymentType, ddlPaymentType.SelectedValue ?? ddlPaymentType.Items[0].Value}
                    }
                    : null;
            }
            set
            {
                txtShopID.Text = value.ElementOrDefault(YandexMoneyTemplate.ShopId);
                txtScID.Text = value.ElementOrDefault(YandexMoneyTemplate.ScId);
                txtCurrencyValue.Text = value.ElementOrDefault(YandexMoneyTemplate.CurrencyValue);

                if (ddlPaymentType.Items.FindByValue(value.ElementOrDefault(YandexMoneyTemplate.YaPaymentType)) != null)
                {
                    ddlPaymentType.SelectedValue = value.ElementOrDefault(YandexMoneyTemplate.YaPaymentType);
                }
            }
        }
    }
}