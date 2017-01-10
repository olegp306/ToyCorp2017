using System;
using System.Drawing;
using AdvantShop;
using AdvantShop.Modules;

namespace Advantshop.Modules.ShoppingCartPopup
{
    public partial class Admin_ShoppingCartPopupModule : System.Web.UI.UserControl
    {
        private const string _moduleName = "ShoppingCartPopup";

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var showMode = ModuleSettingsProvider.GetSettingValue<string>("showmode", _moduleName);
                if (ddlShowMode.Items.FindByValue(showMode) != null)
                {
                    ddlShowMode.SelectedValue = showMode;
                }
                else
                {
                    ddlShowMode.SelectedIndex = 0;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            ModuleSettingsProvider.SetSettingValue("showmode", ddlShowMode.SelectedValue, _moduleName);

            lblMessage.Text = (String)GetLocalResourceObject("ShoppingCartPopup_Message");
            lblMessage.ForeColor = Color.Blue;
            lblMessage.Visible = true;
        }
    }
}