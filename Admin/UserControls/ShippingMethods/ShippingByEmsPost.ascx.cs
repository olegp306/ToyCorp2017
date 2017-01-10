using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Shipping;

namespace Admin.UserControls.ShippingMethods
{
    public partial class ShippingByEmsPostControl : ParametersControl
    {

        protected void Page_Init(object sender, EventArgs e)
        {
            ddlShippingCityFrom.Items.AddRange(ShippingEmsPostService.GetCities().Select(city => new ListItem(city.name, city.value)).ToArray());
            ddlShippingCityFrom.Items.AddRange(ShippingEmsPostService.GetRegions().Select(city => new ListItem(city.name, city.value)).ToArray());
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                var maxWeight = ShippingEmsPostService.GetMaxWeight();
                
                return _valid ||
                       ValidateFormData(new[] {txtShippingWeight, txtExtraPrice},
                                        new[] {txtShippingWeight, txtExtraPrice},
                                        new TextBox[0])
                    ? new Dictionary<string, string>
                    {
                        {ShippingByEmsPostTemplate.CityFrom, ddlShippingCityFrom.SelectedValue},
                        {ShippingByEmsPostTemplate.DefaultWeight, txtShippingWeight.Text},
                        {ShippingByEmsPostTemplate.ExtraPrice, txtExtraPrice.Text},
                        {ShippingByEmsPostTemplate.MaxWeight, maxWeight != 0 ? maxWeight.ToString() : 31.5.ToString()}
                    }
                    : null;
            }
            set
            {
                ddlShippingCityFrom.SelectedValue = value.ElementOrDefault(ShippingByEmsPostTemplate.CityFrom);
                txtShippingWeight.Text = value.ElementOrDefault(ShippingByEmsPostTemplate.DefaultWeight) ?? "1";
                txtExtraPrice.Text = value.ElementOrDefault(ShippingByEmsPostTemplate.ExtraPrice) ?? "0";
                lblMaxWeight.Text = value.ElementOrDefault(ShippingByEmsPostTemplate.MaxWeight);
            }
        }
    }
}