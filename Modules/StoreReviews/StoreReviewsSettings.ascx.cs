using System;
using System.Drawing;
using AdvantShop.Modules;

namespace Advantshop.UserControls.Modules.StoreReviews
{
    public partial class Admin_StoreReviewsSettings : System.Web.UI.UserControl
    {
        private const string _moduleName = "StoreReviews";

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            //ckbEnableStoreReviews.Checked = ModuleSettingsProvider.GetSettingValue<bool>("EnableStoreReviews", _moduleName);
            chkShowRatio.Checked = ModuleSettingsProvider.GetSettingValue<bool>("ShowRatio", _moduleName);
            ckbActiveModerate.Checked = ModuleSettingsProvider.GetSettingValue<bool>("ActiveModerateStoreReviews", _moduleName);
            txtPageSize.Text = ModuleSettingsProvider.GetSettingValue<string>("PageSize", _moduleName);

            txtPageTitle.Text = ModuleSettingsProvider.GetSettingValue<string>("PageTitle", _moduleName);
            txtMetaDescription.Text = ModuleSettingsProvider.GetSettingValue<string>("MetaDescription", _moduleName);
            txtMetaKeyWords.Text = ModuleSettingsProvider.GetSettingValue<string>("MetaKeyWords", _moduleName);
        }

        protected void Save()
        {
            //ModuleSettingsProvider.SetSettingValue("EnableStoreReviews", ckbEnableStoreReviews.Checked, _moduleName);
            ModuleSettingsProvider.SetSettingValue("ShowRatio", chkShowRatio.Checked, _moduleName);
            ModuleSettingsProvider.SetSettingValue("ActiveModerateStoreReviews", ckbActiveModerate.Checked, _moduleName);
            ModuleSettingsProvider.SetSettingValue("PageSize", txtPageSize.Text, _moduleName);

            ModuleSettingsProvider.SetSettingValue("PageTitle", txtPageTitle.Text, _moduleName);
            ModuleSettingsProvider.SetSettingValue("MetaDescription", txtMetaDescription.Text, _moduleName);
            ModuleSettingsProvider.SetSettingValue("MetaKeyWords", txtMetaKeyWords.Text, _moduleName);

            lblMessage.Text = (string)GetLocalResourceObject("StoreReviews_ChangesSaved");
            lblMessage.ForeColor = Color.Blue;
            lblMessage.Visible = true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int pageSize;
            bool resultParsePageSize = int.TryParse(txtPageSize.Text, out pageSize);

            if (!resultParsePageSize)
            {
                lblMessage.Text = (string)GetLocalResourceObject("StoreReviews_SaveErrorPageSize");
                lblMessage.ForeColor = Color.Red;
                lblMessage.Visible = true;
            }
            else
            {
                Save();
            }
        }
    }
}