using System;
using System.Drawing;
using AdvantShop;
using AdvantShop.BonusSystem;
using AdvantShop.Helpers;
using AdvantShop.Trial;

namespace Advantshop.Modules.BonusSystem
{
    public partial class Admin_BonusSystemModule : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            txtApiKey.Text = AdvantShop.BonusSystem.BonusSystem.ApiKey;
            ddlBonusType.SelectedValue = ((int)AdvantShop.BonusSystem.BonusSystem.BonusType).ToString();
            lblBonusFirstPercent.Text = AdvantShop.BonusSystem.BonusSystem.BonusFirstPercent.ToString();
            txtMaxOrderPercent.Text = AdvantShop.BonusSystem.BonusSystem.MaxOrderPercent.ToString("F2");
            txtBonusesForNewCard.Text = AdvantShop.BonusSystem.BonusSystem.BonusesForNewCard.ToString("F2");

            ckbUseOrderId.Checked = AdvantShop.BonusSystem.BonusSystem.UseOrderId;


            if (TrialService.IsTrialEnabled)
            {
                txtApiKey.Visible = false;
                divTrial.Visible = true;
            }
            else
            {
                txtApiKey.Visible = true;
                divTrial.Visible = false;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            AdvantShop.BonusSystem.BonusSystem.ApiKey = txtNewKey.Text.IsNotEmpty() ? txtNewKey.Text : txtApiKey.Text;
            AdvantShop.BonusSystem.BonusSystem.BonusType = (AdvantShop.BonusSystem.BonusSystem.EBonusType)SQLDataHelper.GetInt(ddlBonusType.SelectedValue);
            AdvantShop.BonusSystem.BonusSystem.MaxOrderPercent = txtMaxOrderPercent.Text.TryParseFloat(100);
            AdvantShop.BonusSystem.BonusSystem.BonusesForNewCard = txtBonusesForNewCard.Text.TryParseFloat();

            AdvantShop.BonusSystem.BonusSystem.UseOrderId = ckbUseOrderId.Checked;

            if (BonusSystemService.IsActive())
            {
                lblMessage.Text = (String)GetLocalResourceObject("BonusSystem_Message");
                lblMessage.ForeColor = Color.Blue;
                lblMessage.Visible = true;
            }
            else
            {
                lblMessage.Text = (String)GetLocalResourceObject("BonusSystem_Save_Error");
                lblMessage.ForeColor = Color.Red;
                lblMessage.Visible = true;
            }
        }
    }
}