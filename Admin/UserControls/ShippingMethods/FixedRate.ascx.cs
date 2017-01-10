using System.Collections.Generic;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Shipping;

namespace Admin.UserControls.ShippingMethods
{
    public partial class FixedRateControl : ParametersControl
    {

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return _valid || ValidateFormData(new[] { txtShippingPrice, txtExtracharge },
                                                  new[] { txtShippingPrice, txtExtracharge },
                                                  new TextBox[0])
                           ? new Dictionary<string, string>
                               {
                                   {FixeRateShippingTemplate.ShippingPrice, txtShippingPrice.Text},
                                   {FixeRateShippingTemplate.Extracharge, txtExtracharge.Text},
                                   {FixeRateShippingTemplate.DeliveryTime, txtDeliveryTime.Text},
                               }
                           : null;
            }
            set
            {
                txtExtracharge.Text = value.ElementOrDefault(FixeRateShippingTemplate.Extracharge);
                txtShippingPrice.Text = value.ElementOrDefault(FixeRateShippingTemplate.ShippingPrice);
                txtDeliveryTime.Text = value.ElementOrDefault(FixeRateShippingTemplate.DeliveryTime);
            }
        }
    }
}