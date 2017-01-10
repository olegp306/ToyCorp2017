using System.Collections.Generic;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Shipping;

namespace Admin.UserControls.ShippingMethods
{
    public partial class ShippingByWeightControl : ParametersControl
    {
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return _valid || ValidateFormData(new[] {txtPricePerKg, txtExtracharge,},
                                                  new[] {txtPricePerKg, txtExtracharge,},
                                                  new TextBox[0])
                           ? new Dictionary<string, string>
                               {
                                   {ShippingByWeightTemplate.PricePerKg, txtPricePerKg.Text},
                                   {ShippingByWeightTemplate.Extracharge, txtExtracharge.Text},
                                   {ShippingByWeightTemplate .DeliveryTime, txtDeliveryTime.Text }
                               }
                           : null;
            }
            set
            {
                txtExtracharge.Text = value.ElementOrDefault(ShippingByWeightTemplate.Extracharge);
                txtPricePerKg.Text = value.ElementOrDefault(ShippingByWeightTemplate.PricePerKg);
                txtDeliveryTime.Text = value.ElementOrDefault(ShippingByWeightTemplate.DeliveryTime);
            }
        }
    }
}