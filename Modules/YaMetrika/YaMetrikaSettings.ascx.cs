using System;
using System.Drawing;
using AdvantShop.Modules;

namespace Advantshop.Modules.YaMetrika.UserControls
{
    public partial class Admin_YaMetrikaSettings : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            txtCounterId.Text = ModuleSettingsProvider.GetSettingValue<string>("COUNTER_ID", AdvantShop.Modules.YaMetrika.ModuleID);
            txtCounter.Text = ModuleSettingsProvider.GetSettingValue<string>("COUNTER", AdvantShop.Modules.YaMetrika.ModuleID);
        }

        protected void Save()
        {
            ModuleSettingsProvider.SetSettingValue("COUNTER_ID", txtCounterId.Text, AdvantShop.Modules.YaMetrika.ModuleID);
            ModuleSettingsProvider.SetSettingValue("COUNTER", txtCounter.Text, AdvantShop.Modules.YaMetrika.ModuleID);

            lblMessage.Text = (string)GetLocalResourceObject("YaMetrika_ChangesSaved");
            lblMessage.ForeColor = Color.Blue;
            lblMessage.Visible = true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Save();
        }
    }
}