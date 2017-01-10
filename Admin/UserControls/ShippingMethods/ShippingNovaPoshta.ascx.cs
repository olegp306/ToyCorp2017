using System;
using System.Collections.Generic;
using System.Globalization;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Helpers;
using AdvantShop.Shipping;

namespace Admin.UserControls.ShippingMethods
{
    public partial class ShippingNovaPoshtaControl : ParametersControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            rvRate.MinimumValue = float.Parse("0.0001", CultureInfo.InvariantCulture).ToString();
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return _valid || ValidateFormData(new[] { txtAPIKey, txtCityFrom, txtRate }, new[] { txtRate })
                           ? new Dictionary<string, string>
                               {
                                   {NovaPoshtaTemplate.APIKey, txtAPIKey.Text},
                                   {NovaPoshtaTemplate.CityFrom, txtCityFrom.Text},
                                   {NovaPoshtaTemplate.DeliveryType , ddlDeliveryType.SelectedValue },
                                   //{NovaPoshtaTemplate.EnabledInsurance, cbEnabledInsurance.Checked.ToString() },
                                   //{NovaPoshtaTemplate.EnabledCOD, chbcreateCOD.Checked.ToString() },
                                   //{NovaPoshtaTemplate.ShipIdCOD, hfCod.Value },
                                   {NovaPoshtaTemplate.Rate, txtRate.Text }
                               }
                           : null;
            }
            set
            {
                txtAPIKey.Text = value.ElementOrDefault(NovaPoshtaTemplate.APIKey);
                txtCityFrom.Text = value.ElementOrDefault(NovaPoshtaTemplate.CityFrom);
                ddlDeliveryType.SelectedValue = value.ElementOrDefault(NovaPoshtaTemplate.DeliveryType);
                //cbEnabledInsurance.Checked = SQLDataHelper.GetBoolean(value.ElementOrDefault(NovaPoshtaTemplate.EnabledInsurance));
                txtRate.Text = value.ElementOrDefault(NovaPoshtaTemplate.Rate) ?? "1";

                //chbcreateCOD.Checked = SQLDataHelper.GetBoolean(value.ElementOrDefault(NovaPoshtaTemplate.EnabledCOD));
                //hfCod.Value = value.ElementOrDefault(NovaPoshtaTemplate.ShipIdCOD);
            }
        }
    }
}