using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Shipping;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Admin.UserControls.ShippingMethods
{
    public partial class ShippingByRangeWeightAndDistanceControl : ParametersControl
    {

        private List<ShippingByRangeWeightAndDistance.WeightLimit> WeightLimits
        {
            get { return ((List<ShippingByRangeWeightAndDistance.WeightLimit>)ViewState["WeightLimits"]); }
            set { ViewState["WeightLimits"] = value; }
        }

        private List<ShippingByRangeWeightAndDistance.DistanceLimit> DistanceLimits
        {
            get { return ((List<ShippingByRangeWeightAndDistance.DistanceLimit>)ViewState["DistanceLimits"]); }
            set { ViewState["DistanceLimits"] = value; }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                    {
                        {ShippingByRangeWeightAndDistanceTemplate.WeightLimit,JsonConvert.SerializeObject(WeightLimits)},
                        {ShippingByRangeWeightAndDistanceTemplate.DistanceLimit,JsonConvert.SerializeObject(DistanceLimits)},
                        {ShippingByRangeWeightAndDistanceTemplate.UseDistance,chbUseDistance.Checked.ToString()},
                        {ShippingByRangeWeightAndDistanceTemplate.DeliveryTime, txtDeliveryTime.Text }
                    };

            }
            set
            {
                WeightLimits = value.ElementOrDefault(ShippingByRangeWeightAndDistanceTemplate.WeightLimit) == null ? new List<ShippingByRangeWeightAndDistance.WeightLimit>() : JsonConvert.DeserializeObject<List<ShippingByRangeWeightAndDistance.WeightLimit>>(value.ElementOrDefault(ShippingByRangeWeightAndDistanceTemplate.WeightLimit));
                DistanceLimits = value.ElementOrDefault(ShippingByRangeWeightAndDistanceTemplate.DistanceLimit) == null ? new List<ShippingByRangeWeightAndDistance.DistanceLimit>() : JsonConvert.DeserializeObject<List<ShippingByRangeWeightAndDistance.DistanceLimit>>(value.ElementOrDefault(ShippingByRangeWeightAndDistanceTemplate.DistanceLimit));
                chbUseDistance.Checked = value.ElementOrDefault(ShippingByRangeWeightAndDistanceTemplate.UseDistance).TryParseBool();
                txtDeliveryTime.Text = value.ElementOrDefault(ShippingByOrderPriceTemplate.DeliveryTime);
                BindRepeater();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            WeightLimits.Add(new ShippingByRangeWeightAndDistance.WeightLimit()
                {
                    Amount = txtAmount.Text.TryParseFloat(),
                    Price = txtPrice.Text.TryParseFloat(),
                    PerUnit = chbShippingPerUnit.Checked,
                });

            BindRepeater();
            txtAmount.Text = "";
            txtPrice.Text = "";
            chbShippingPerUnit.Checked = false;
        }

        protected void rWeightLimits_Delete(object sender, RepeaterCommandEventArgs e)
        {
            var w = WeightLimits.OrderBy(x => x.Amount).ToList();
            w.RemoveAt(e.CommandArgument.ToString().TryParseInt());
            WeightLimits = w;
            BindRepeater();
        }

        private void BindRepeater()
        {
            rWeightLimits.DataSource = WeightLimits.OrderBy(item => item.Amount);
            rWeightLimits.DataBind();
            rDistanceLimits.DataSource = DistanceLimits.OrderBy(item => item.Amount);
            rDistanceLimits.DataBind();
        }

        protected void btnAddD_Click(object sender, EventArgs e)
        {
            DistanceLimits.Add(new ShippingByRangeWeightAndDistance.DistanceLimit()
                {
                    Amount = txtDistanse.Text.TryParseFloat(),
                    PerUnit = chbPerUnit.Checked,
                    Price = txtDistansePrice.Text.TryParseFloat()
                });

            BindRepeater();
            txtDistanse.Text = "";
            chbPerUnit.Checked = false;
            txtDistansePrice.Text = "";
        }

        protected void rDistanceLimits_Delete(object source, RepeaterCommandEventArgs e)
        {
            var d = DistanceLimits.OrderBy(x => x.Amount).ToList();
            d.RemoveAt(e.CommandArgument.ToString().TryParseInt());
            DistanceLimits = d;
            BindRepeater();
        }
    }
}