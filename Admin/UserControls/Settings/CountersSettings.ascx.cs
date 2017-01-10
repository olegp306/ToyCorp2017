using System;
using AdvantShop.Configuration;
using AdvantShop.Trial;

namespace Admin.UserControls.Settings
{
    public partial class CountersSettings : System.Web.UI.UserControl
    {
        public string ErrMessage = Resources.Resource.Admin_CommonSettings_InvalidSEO;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            txtGoogleAnalytics.Text = SettingsSEO.GoogleAnalyticsNumber;
            chkGaUseDemografic.Checked = SettingsSEO.GoogleAnalyticsEnableDemogrReports;
            chbGoogleAnalytics.Checked = SettingsSEO.GoogleAnalyticsEnabled;

            chbGoogleAnalyticsApi.Checked = SettingsSEO.GoogleAnalyticsApiEnabled;
            txtGoogleAnalyticsAccountID.Text = SettingsSEO.GoogleAnalyticsAccountID;
            txtGoogleAnalyticsUserName.Text = SettingsSEO.GoogleAnalyticsUserName;
            txtGoogleAnalyticsPassword.Text = SettingsSEO.GoogleAnalyticsPassword;
            txtGoogleAnalyticsAPIKey.Text = SettingsSEO.GoogleAnalyticsAPIKey;

            chbUseGTM.Checked = SettingsSEO.UseGTM;
            txtGTMContainerID.Text = SettingsSEO.GTMContainerID;

            txtOrderSuccessScript.Text = SettingsOrderConfirmation.SuccessOrderScript;

        }
        public bool SaveData()
        {
            if (SettingsSEO.GoogleAnalyticsNumber != txtGoogleAnalytics.Text && chbGoogleAnalytics.Checked)
            {
                TrialService.TrackEvent(TrialEvents.SetUpGoogleAnalytics, string.Empty);
            }

            SettingsSEO.GoogleAnalyticsNumber = txtGoogleAnalytics.Text;
            SettingsSEO.GoogleAnalyticsEnableDemogrReports = chkGaUseDemografic.Checked;
            SettingsSEO.GoogleAnalyticsEnabled = chbGoogleAnalytics.Checked;

            SettingsSEO.GoogleAnalyticsApiEnabled = chbGoogleAnalyticsApi.Checked; 
            SettingsSEO.GoogleAnalyticsAccountID = txtGoogleAnalyticsAccountID.Text;
            SettingsSEO.GoogleAnalyticsUserName = txtGoogleAnalyticsUserName.Text;
            SettingsSEO.GoogleAnalyticsPassword = txtGoogleAnalyticsPassword.Text;
            SettingsSEO.GoogleAnalyticsAPIKey = txtGoogleAnalyticsAPIKey.Text;

            SettingsSEO.UseGTM = chbUseGTM.Checked;
            SettingsSEO.GTMContainerID = txtGTMContainerID.Text;

            SettingsOrderConfirmation.SuccessOrderScript = txtOrderSuccessScript.Text;

            LoadData();

            return true;
        }
    }
}