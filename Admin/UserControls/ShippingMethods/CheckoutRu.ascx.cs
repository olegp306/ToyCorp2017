using System;
using System.Collections.Generic;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Helpers;
using AdvantShop.Shipping;

public partial class Admin_UserControls_ShippingMethods_CheckoutRu : ParametersControl
{
    public override Dictionary<string, string> Parameters
    {
        get
        {

            return _valid || ValidateFormData(new[] { txtClientId })
                         ? new Dictionary<string, string>
                               {
                                   {ShippingCheckoutRuTemplate.ClientId, txtClientId.Text.Trim()},
                                   {ShippingCheckoutRuTemplate.Grouping, ckbGrouping.Checked.ToString()},
                                   {ShippingCheckoutRuTemplate.EnabledCOD, chbcreateCOD.Checked.ToString()},
                                   {ShippingCheckoutRuTemplate.ShipIdCOD, hfCod.Value},
                                   {ShippingCheckoutRuTemplate.ExtrachargeType, ddlExtrachargeType.SelectedValue},
                                   {ShippingCheckoutRuTemplate.Extracharge, txtExtracharge.Text.TryParseFloat().ToString("F2")}
                               }
                         : null;

        }
        set
        {
            txtClientId.Text = value.ElementOrDefault(ShippingCheckoutRuTemplate.ClientId);
            ckbGrouping.Checked = Convert.ToBoolean(value.ElementOrDefault(ShippingCheckoutRuTemplate.Grouping));
            chbcreateCOD.Checked = SQLDataHelper.GetBoolean(value.ElementOrDefault(ShippingCheckoutRuTemplate.EnabledCOD));
            hfCod.Value = value.ElementOrDefault(ShippingCheckoutRuTemplate.ShipIdCOD);
            ddlExtrachargeType.SelectedValue = value.ElementOrDefault(ShippingCheckoutRuTemplate.ExtrachargeType);
            txtExtracharge.Text = value.ElementOrDefault(ShippingCheckoutRuTemplate.Extracharge);
        }
    }
}