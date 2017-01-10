using System;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using AdvantShop.Permission;
using AdvantShop.SaasData;

namespace Admin.UserControls.Settings
{
    public partial class LicSettings : System.Web.UI.UserControl
    {
        public string ErrMessage = Resources.Resource.Admin_CommonSettings_InvalidLic;

        protected void Page_Load(object sender, EventArgs e)
        {
            txtLicKey.Enabled = btnCheakLic.Enabled = !SaasDataService.IsSaasEnabled;
            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            txtLicKey.Text = SettingsLic.LicKey;
            lblState.Text = SettingsLic.ActiveLic ? Resources.Resource.Admin_UserControls_Settings_LicSettings_Active : Resources.Resource.Admin_UserControls_Settings_LicSettings_Deactive;
        }

        public bool SaveData()
        {
            SettingsLic.LicKey = txtLicKey.Text;

            LoadData();
            return true;
        }

        protected void btnCheakLic_Click(object sender, EventArgs e)
        {
            try
            {
                SettingsLic.ActiveLic = PermissionAccsess.ActiveLic(txtLicKey.Text, SettingsMain.SiteUrl,
                                                                    SettingsMain.ShopName, SettingsGeneral.SiteVersion, SettingsGeneral.SiteVersionDev);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex, "Error at license check at settings");
            }
            SettingsLic.LicKey = txtLicKey.Text;
            LoadData();
        }
    }
}