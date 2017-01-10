using System;
using System.Linq;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Localization;
using AdvantShop.Orders;

namespace Admin.UserControls.Settings
{
    public partial class BankSettings : System.Web.UI.UserControl
    {
        private bool _isRu = Culture.Language == Culture.SupportLanguage.Russian ||
                             Culture.Language == Culture.SupportLanguage.Ukrainian;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            var apikey = SettingsApi.ApiKey;
            var siteUrl = SettingsMain.SiteUrl.TrimEnd('/');

            txtApiKey.Text = apikey;

            if (_isRu)
            {
                tb1cApi.Visible = true;

                lblImportProductsUrl.Text = siteUrl + "/api/1c/importproducts.ashx?apikey=" + apikey;
                lblImportPhotosUrl.Text = siteUrl + "/api/1c/importphotos.ashx?apikey=" + apikey;
                lblExportProductsUrl.Text = siteUrl + "/api/1c/exportproducts.ashx?apikey=" + apikey;
                lblExportOrdersUrl.Text = siteUrl + "/api/1c/exportorders.ashx?apikey=" + apikey;
                lblChangeOrderStatusUrl.Text = siteUrl + "/api/1c/changeorderstatus.ashx?apikey=" + apikey;
                lblDeletedOrdersUrl.Text = siteUrl + "/api/1c/deletedorders.ashx?apikey=" + apikey;
                lblDeletedProducts.Text = siteUrl + "/api/1c/deletedproducts.ashx?apikey=" + apikey;

                chk1CEnabled.Checked = Settings1C.Enabled;
                ddlExportOrdersType.SelectedValue = Settings1C.OnlyUseIn1COrders ? "0" : "1";

                //var statuses = OrderService.GetOrderStatuses();

                //ddlNotAssigned.DataSource = statuses;
                //ddlNotAssigned.DataBind();

                //if (ddlNotAssigned.Items.FindByValue(Settings1C.NotAssignedStatus) != null)
                //    ddlNotAssigned.SelectedValue = Settings1C.NotAssignedStatus;

                //ddlAssigned.DataSource = statuses;
                //ddlAssigned.DataBind();

                //if (ddlAssigned.Items.FindByValue(Settings1C.AssignedStatus) != null)
                //    ddlAssigned.SelectedValue = Settings1C.AssignedStatus;

                //ddlRezerv.DataSource = statuses;
                //ddlRezerv.DataBind();

                //if (ddlRezerv.Items.FindByValue(Settings1C.RezervStatus) != null)
                //    ddlRezerv.SelectedValue = Settings1C.RezervStatus;

                //ddlToShip.DataSource = statuses;
                //ddlToShip.DataBind();

                //if (ddlToShip.Items.FindByValue(Settings1C.ToShipStatus) != null)
                //    ddlToShip.SelectedValue = Settings1C.ToShipStatus;

                //ddlClosed.DataSource = statuses;
                //ddlClosed.DataBind();

                //if (ddlClosed.Items.FindByValue(Settings1C.ClosedStatus) != null)
                //    ddlClosed.SelectedValue = Settings1C.ClosedStatus;
            }
        }

        public bool SaveData()
        {
            SettingsApi.ApiKey = txtApiKey.Text.Trim();

            if (_isRu)
            {
                Settings1C.Enabled = chk1CEnabled.Checked;
                Settings1C.OnlyUseIn1COrders = ddlExportOrdersType.SelectedValue == "0";

                //Settings1C.AssignedStatus = ddlAssigned.SelectedValue;
                //Settings1C.NotAssignedStatus = ddlNotAssigned.SelectedValue;
                //Settings1C.RezervStatus = ddlRezerv.SelectedValue;
                //Settings1C.ToShipStatus = ddlToShip.SelectedValue;
                //Settings1C.ClosedStatus = ddlClosed.SelectedValue;
            }

            LoadData();
            return true;
        }
        

        protected void lbGenerateApiKey_Click(object sender, EventArgs e)
        {
            txtApiKey.Text = Guid.NewGuid().ToString().Sha256();

            SaveData();
        }
    }
}