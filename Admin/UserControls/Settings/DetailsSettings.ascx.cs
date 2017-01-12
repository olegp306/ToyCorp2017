using System;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Helpers;
using Resources;

namespace Admin.UserControls.Settings
{
    public partial class DetailsSettings : System.Web.UI.UserControl
    {
        public string ErrMessage = Resource.Admin_CommonSettings_InvalidCatalog;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {

            chkDisplayWeight.Checked = SettingsCatalog.DisplayWeight;
            chkDisplayDimensions.Checked = SettingsCatalog.DisplayDimensions;
            cbShowStockAvailability.Checked = SettingsCatalog.ShowStockAvailability;
            cbShowBlockStockAvailability.Checked = SettingsCatalog.ShowBlockStockAvailability;

            ckbModerateReviews.Checked = SettingsCatalog.ModerateReviews;
            chkAllowReviews.Checked = SettingsCatalog.AllowReviews;
            chkCompressBigImage.Checked = SettingsCatalog.CompressBigImage;

            chkEnableZoom.Checked = SettingsDesign.EnableZoom;

            ddlShowShippingsMethodsInDetails.SelectedValue =
                ((int) SettingsDesign.ShowShippingsMethodsInDetails).ToString();

            txtShippingsMethodsInDetailsCount.Text = SettingsDesign.ShippingsMethodsInDetailsCount.ToString();
        }


        public bool SaveData()
        {
            if (!ValidateData())
                return false;

            SettingsCatalog.DisplayWeight = chkDisplayWeight.Checked;
            SettingsCatalog.DisplayDimensions = chkDisplayDimensions.Checked;
            SettingsCatalog.ShowStockAvailability = cbShowStockAvailability.Checked;
            SettingsCatalog.ShowBlockStockAvailability = cbShowBlockStockAvailability.Checked;

            SettingsCatalog.CompressBigImage = chkCompressBigImage.Checked;
            SettingsCatalog.ModerateReviews = ckbModerateReviews.Checked;
            SettingsCatalog.AllowReviews = chkAllowReviews.Checked;
            
            SettingsDesign.EnableZoom = chkEnableZoom.Checked;

            SettingsDesign.ShowShippingsMethodsInDetails =
                (SettingsDesign.eShowShippingsInDetails)
                    SQLDataHelper.GetInt(ddlShowShippingsMethodsInDetails.SelectedValue);

            SettingsDesign.ShippingsMethodsInDetailsCount = txtShippingsMethodsInDetailsCount.Text.TryParseInt();

            return true;
        }

        private bool ValidateData()
        {
            return true;
        }
    }
}