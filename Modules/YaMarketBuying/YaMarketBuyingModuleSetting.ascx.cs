using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Modules;
using AdvantShop.Orders;
using AdvantShop.Shipping;

namespace Advantshop.Modules.YaMarketBuyingModuleSetting
{
    public partial class Admin_YaMarketBuyingModuleSetting : System.Web.UI.UserControl
    {
        protected void Page_PreRender(object sender, EventArgs e)
        {
            LoadSettings();
        }

        protected void lvShippings_OnItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var item = (YaMarketShipping)e.Item.DataItem;

                if (!string.IsNullOrEmpty(item.Type))
                {
                    ((DropDownList)e.Item.FindControl("ddlType")).SelectedValue = item.Type;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void LoadSettings()
        {
            lblApiUrl.Text = SettingsMain.SiteUrl.TrimEnd('/').Replace("http", "https") + "/" + YaMarketByuingApiService.ApiUrl;
            txtAuth.Text = YaMarketBuyingSettings.AuthToken;

            

            var shipppings = new List<YaMarketShipping>();
            var shippingMethods = ShippingMethodService.GetAllShippingMethods(true);
            var selshippings = YaMarketByuingService.GetShippings();

            foreach (var shipping in shippingMethods)
            {
                var ship = new YaMarketShipping() { Name = shipping.Name, ShippingMethodId = shipping.ShippingMethodId };

                var sel = selshippings.FirstOrDefault(x => x.ShippingMethodId == shipping.ShippingMethodId);
                if (sel != null)
                {
                    ship.Type = sel.Type;
                    ship.MinDate = sel.MinDate;
                    ship.MaxDate = sel.MaxDate;
                }
                shipppings.Add(ship);
            }

            lvShippings.DataSource = shipppings;
            lvShippings.DataBind();

            var payments = YaMarketBuyingSettings.Payments.Split(';');

            ddlYandex.SelectedValue = payments.Any(x => x == "YANDEX") ? "1" : "0";
            ddlShopprepaid.SelectedValue = payments.Any(x => x == "SHOP_PREPAID") ? "1" : "0";
            ddlCashOnDelivery.SelectedValue = payments.Any(x => x == "CASH_ON_DELIVERY") ? "1" : "0";
            ddlCardOnDelivery.SelectedValue = payments.Any(x => x == "CARD_ON_DELIVERY") ? "1" : "0";

            ddlUnpaidStatus.Items.Clear();
            ddlUnpaidStatus.Items.AddRange(OrderService.GetOrderStatuses().Select(x => new ListItem(x.StatusName, x.StatusID.ToString())).ToArray());

            ddlProcessingStatus.Items.Clear();
            ddlProcessingStatus.Items.AddRange(OrderService.GetOrderStatuses().Select(x => new ListItem(x.StatusName, x.StatusID.ToString())).ToArray());

            ddlDeliveryStatus.Items.Clear();
            ddlDeliveryStatus.Items.AddRange(OrderService.GetOrderStatuses().Select(x => new ListItem(x.StatusName, x.StatusID.ToString())).ToArray());

            ddlDeliveredStatus.Items.Clear();
            ddlDeliveredStatus.Items.AddRange(OrderService.GetOrderStatuses().Select(x => new ListItem(x.StatusName, x.StatusID.ToString())).ToArray());

            if (ddlUnpaidStatus.Items.FindByValue(YaMarketBuyingSettings.UpaidStatusId.ToString()) != null)
                ddlUnpaidStatus.SelectedValue = YaMarketBuyingSettings.UpaidStatusId.ToString();

            if (ddlProcessingStatus.Items.FindByValue(YaMarketBuyingSettings.ProcessingStatusId.ToString()) != null)
                ddlProcessingStatus.SelectedValue = YaMarketBuyingSettings.ProcessingStatusId.ToString();

            if (ddlDeliveryStatus.Items.FindByValue(YaMarketBuyingSettings.DeliveryStatusId.ToString()) != null)
                ddlDeliveryStatus.SelectedValue = YaMarketBuyingSettings.DeliveryStatusId.ToString();

            if (ddlDeliveredStatus.Items.FindByValue(YaMarketBuyingSettings.DeliveredStatusId.ToString()) != null)
                ddlDeliveredStatus.SelectedValue = YaMarketBuyingSettings.DeliveredStatusId.ToString();

            txtCampaignId.Text = YaMarketBuyingSettings.CampaignId;
            txtAuthTokenToYaMarket.Text = YaMarketBuyingSettings.AuthTokenToMarket;
            txtAuthClientId.Text = YaMarketBuyingSettings.AuthClientId;
            txtLogin.Text = YaMarketBuyingSettings.Login;
        }

        private void SaveSettings()
        {
            YaMarketByuingService.DeleteShippings();

            var outlets = new List<int>();

            foreach (var dataItem in lvShippings.Items)
            {
                var type = ((DropDownList)dataItem.FindControl("ddlType")).SelectedValue;
                var methodId = ((HiddenField)dataItem.FindControl("hfShippingMethodId")).Value.TryParseInt();
                var minDay = ((TextBox)dataItem.FindControl("txtMinDate")).Text.TryParseInt();
                var maxDay = ((TextBox)dataItem.FindControl("txtMaxDate")).Text.TryParseInt();

                if (type != "")
                {
                    YaMarketByuingService.AddShipping(new YaMarketShipping()
                    {
                        ShippingMethodId = methodId,
                        Type = type,
                        MinDate = minDay,
                        MaxDate = maxDay
                    });
                }

                if (type == "PICKUP")
                    outlets.Add(methodId);
            }

            var payments = new List<string>();

            if (ddlYandex.SelectedValue == "1")
                payments.Add("YANDEX");

            if (ddlShopprepaid.SelectedValue == "1")
                payments.Add("SHOP_PREPAID");

            if (ddlCashOnDelivery.SelectedValue == "1")
                payments.Add("CASH_ON_DELIVERY");

            if (ddlCardOnDelivery.SelectedValue == "1")
                payments.Add("CARD_ON_DELIVERY");

            YaMarketBuyingSettings.Payments = string.Join(";", payments);
            YaMarketBuyingSettings.Outlets = string.Join(";", outlets);
            YaMarketBuyingSettings.AuthToken = txtAuth.Text.Trim();
            YaMarketBuyingSettings.AuthTokenToMarket = txtAuthTokenToYaMarket.Text.Trim();
            YaMarketBuyingSettings.AuthClientId = txtAuthClientId.Text.Trim();
            YaMarketBuyingSettings.Login = txtLogin.Text;

            YaMarketBuyingSettings.UpaidStatusId = ddlUnpaidStatus.SelectedValue.TryParseInt();
            YaMarketBuyingSettings.ProcessingStatusId =  ddlProcessingStatus.SelectedValue.TryParseInt();
            YaMarketBuyingSettings.DeliveryStatusId = ddlDeliveryStatus.SelectedValue.TryParseInt();
            YaMarketBuyingSettings.DeliveredStatusId = ddlDeliveredStatus.SelectedValue.TryParseInt();

            YaMarketBuyingSettings.CampaignId = txtCampaignId.Text.Trim();
        }
    }
}