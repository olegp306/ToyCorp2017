using System;
using AdvantShop.Configuration;

namespace Admin.UserControls.Settings
{
    public partial class SocialSettings : System.Web.UI.UserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            rbSocialShareDefault.Checked = !SettingsSocial.SocialShareCustomEnabled;
            rbSocialShareCustom.Checked = SettingsSocial.SocialShareCustomEnabled;
            txtSocialCustomCode.Text = SettingsSocial.SocialShareCustomCode;

        }
        public bool SaveData()
        {
            //active
            SettingsSocial.SocialShareCustomCode = txtSocialCustomCode.Text;
            SettingsSocial.SocialShareCustomEnabled = rbSocialShareCustom.Checked;

            LoadData();

            return true;
        }
    }
}