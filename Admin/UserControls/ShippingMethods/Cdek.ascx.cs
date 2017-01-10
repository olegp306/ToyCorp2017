//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Helpers;
using AdvantShop.Shipping;
using AdvantShop.Shipping.ShippingCdek;

namespace Admin.UserControls.ShippingMethods
{
    public partial class CdekControl : ParametersControl
    {
        private List<string> activeTariffIds = new List<string>();

        public void Page_PreRender(object sender, EventArgs e)
        {
            lvTariffs.DataSource = Cdek.tariffs.Select(item => new CdekTariff
                {
                    tariffId = item.tariffId,
                    name = item.name,
                    mode = item.mode,
                    active = activeTariffIds.Contains(item.tariffId.ToString())
                });
            lvTariffs.DataBind();
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return _valid || ValidateFormData(new[] { txtAuthLogin, txtAuthPassword, txtAdditionalPrice, txtCityFrom }, new[] { txtAdditionalPrice })
                           ? new Dictionary<string, string>
                               {
                                   {CdekTemplate.AuthLogin, txtAuthLogin.Text.Trim()},
                                   {CdekTemplate.AuthPassword, txtAuthPassword.Text.Trim()},
                                   {CdekTemplate.ActiveTariffs, GetActiveTariffIds()},
                                   {CdekTemplate.AdditionalPrice, txtAdditionalPrice.Text.Trim()},
                                   {CdekTemplate.CityFrom, txtCityFrom.Text.Trim()},
                                   {CdekTemplate.EnabledCOD, chbcreateCOD.Checked.ToString()},
                                   {CdekTemplate.ShipIdCOD, hfCod.Value}
                               }
                           : null;
            }
            set
            {
                txtAuthLogin.Text = value.ElementOrDefault(CdekTemplate.AuthLogin);
                txtAuthPassword.Text = value.ElementOrDefault(CdekTemplate.AuthPassword);
                txtAdditionalPrice.Text = value.ElementOrDefault(CdekTemplate.AdditionalPrice);
                txtCityFrom.Text = value.ElementOrDefault(CdekTemplate.CityFrom);

                chbcreateCOD.Checked = SQLDataHelper.GetBoolean(value.ElementOrDefault(CdekTemplate.EnabledCOD));
                hfCod.Value = value.ElementOrDefault(CdekTemplate.ShipIdCOD);

                var tariffIdsString = value.ElementOrDefault(CdekTemplate.ActiveTariffs);
                if (!string.IsNullOrEmpty(tariffIdsString))
                {
                    var tariffIds = tariffIdsString.Split(new[] { ',' });
                    foreach (var tariffId in tariffIds)
                    {
                        activeTariffIds.Add(tariffId);
                    }
                }
            }
        }

        public string GetActiveTariffIds()
        {
            var result = string.Empty;
            foreach (var item in lvTariffs.Items)
            {
                var ckbActive = (CheckBox)item.FindControl("ckbIsActive");
                if (ckbActive != null && ckbActive.Checked)
                {
                    result += ckbActive.Attributes["data-tariffid"] + ",";
                }
            }
            return result;
        }


        protected void btnCallCourier_OnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSenderCity.Text) || string.IsNullOrEmpty(txtSenderStreet.Text) || string.IsNullOrEmpty(txtSenderHouse.Text)
                || string.IsNullOrEmpty(txtSenderFlat.Text) || string.IsNullOrEmpty(txtSenderPhone.Text) || string.IsNullOrEmpty(txtSenderName.Text)
                || string.IsNullOrEmpty(txtSenderWeght.Text))
            {

                return;
            }

            var shippingMethods = ShippingMethodService.GetShippingMethodByType(ShippingType.Cdek);
            if (shippingMethods.Count == 0)
            {
                lblCallCourier.Text = @"Не удалось получить метод доставки";
                lblCallCourier.ForeColor = System.Drawing.Color.Red;
                return;
            }

            var timeBegin = txtTimeFrom.Text.Split(new[] { ':' });
            if (timeBegin.Length != 2)
            {
                lblCallCourier.Text = @"Неверное время доставки TimeFrom";
                lblCallCourier.ForeColor = System.Drawing.Color.Red;
                return;
            }

            var timeEnd = txtTimeTo.Text.Split(new[] { ':' });
            if (timeEnd.Length != 2)
            {
                lblCallCourier.Text = @"Неверное время доставки TimeTo";
                lblCallCourier.ForeColor = System.Drawing.Color.Red;
                return;
            }

            var date = Convert.ToDateTime(txtDate.Text);
            var dateBegin = new DateTime(date.Year, date.Month, date.Day, Convert.ToInt32(timeBegin[0]), Convert.ToInt32(timeBegin[1]), 0);
            var dateEnd = new DateTime(date.Year, date.Month, date.Day, Convert.ToInt32(timeEnd[0]), Convert.ToInt32(timeEnd[1]), 0);

            CdekStatusAnswer result = (new Cdek(shippingMethods[0].Params)).CallCourier(
                date,
                dateBegin,
                dateEnd,
                txtSenderCity.Text,
                txtSenderStreet.Text,
                txtSenderHouse.Text,
                txtSenderFlat.Text,
                txtSenderPhone.Text,
                txtSenderName.Text,
                txtSenderWeght.Text);

            if (!result.Status)
            {
                txtDate.Text = string.Empty;
                txtTimeFrom.Text = string.Empty;
                txtTimeTo.Text = string.Empty;

                txtSenderCity.Text = string.Empty;
                txtSenderStreet.Text = string.Empty;
                txtSenderHouse.Text = string.Empty;
                txtSenderFlat.Text = string.Empty;
                txtSenderPhone.Text = string.Empty;
                txtSenderName.Text = string.Empty;
                txtSenderWeght.Text = string.Empty;
            }
            lblCallCourier.ForeColor = result.Status ? System.Drawing.Color.Blue : System.Drawing.Color.Red;
            lblCallCourier.Text = result.Message;

        }
    }

}