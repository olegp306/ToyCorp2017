using System;
using System.Drawing;
using AdvantShop.Modules;

namespace Advantshop.Modules.UserControls.StoreReviews
{
    public partial class Admin_StoreReviewsEmailSettings : System.Web.UI.UserControl
    {
        private const string _moduleName = "StoreReviews";

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            ckbEnableSendMails.Checked = ModuleSettingsProvider.GetSettingValue<bool>("EnableSendMails", _moduleName);

            txtFormat.Text = ModuleSettingsProvider.GetSettingValue<string>("Format", _moduleName);
            txtSubject.Text = ModuleSettingsProvider.GetSettingValue<string>("Subject", _moduleName);
            txtEmail.Text = ModuleSettingsProvider.GetSettingValue<string>("Email", _moduleName);
        }

        protected void Save()
        {
            ModuleSettingsProvider.SetSettingValue("EnableSendMails", ckbEnableSendMails.Checked, _moduleName);

            ModuleSettingsProvider.SetSettingValue("Format", txtFormat.Text, _moduleName);
            ModuleSettingsProvider.SetSettingValue("Subject", txtSubject.Text, _moduleName);
            ModuleSettingsProvider.SetSettingValue("Email", txtEmail.Text, _moduleName);

            lblMessage.Text = (string)GetLocalResourceObject("StoreReviewsMails_ChangesSaved");
            lblMessage.ForeColor = Color.Blue;
            lblMessage.Visible = true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Save();
        }
    }
}