using System;
using AdvantShop.Configuration;

namespace Admin.UserControls.Settings
{
    public partial class NotifyEmailsSettings : System.Web.UI.UserControl
    {
        public string ErrMessage = Resources.Resource.Admin_CommonSettings_InvalidNotify;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            txtOrderEmail.Text = SettingsMail.EmailForOrders;
            txtEmailProductDiscuss.Text = SettingsMail.EmailForProductDiscuss;
            txtEmailRegReport.Text = SettingsMail.EmailForRegReport;
            txtFeedbackEmail.Text = SettingsMail.EmailForFeedback;
        }

        public bool SaveData()
        {
            if (!ValidateData())
                return false;

            SettingsMail.EmailForOrders = txtOrderEmail.Text;
            SettingsMail.EmailForProductDiscuss = txtEmailProductDiscuss.Text;
            SettingsMail.EmailForRegReport = txtEmailRegReport.Text;
            SettingsMail.EmailForFeedback = txtFeedbackEmail.Text;

            LoadData();

            return true;
        }

        private bool ValidateData()
        {
            if (string.IsNullOrEmpty(txtOrderEmail.Text))
            {
                ErrMessage = "";
                return false;
            }
            return true;
        }
    }
}