using System;
using System.Web.UI.WebControls;
using AdvantShop.Helpers;
using AdvantShop.Modules;

namespace Advantshop.Modules.UserControls.BuyInTime
{
    public partial class Admin_BuyInTimeModule : System.Web.UI.UserControl
    {
        private const string _moduleName = "BuyInTime";
        
        private void MsgErr(string msg)
        {
            lblMessage.Visible = true;
            lblMessage.Text = msg;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
        }
        
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ckeActionTitle.Text = ModuleSettingsProvider.GetSettingValue<string>("BuyInTimeActionTitle", _moduleName);
                txtLabelCode.Text = ModuleSettingsProvider.GetSettingValue<string>("BuyInTimeLabel", _moduleName);
            }

            rprProducts.DataSource = BuyInTimeService.GetProductsTable();
            rprProducts.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            ModuleSettingsProvider.SetSettingValue("BuyInTimeActionTitle", ckeActionTitle.Text, _moduleName);
            ModuleSettingsProvider.SetSettingValue("BuyInTimeLabel", txtLabelCode.Text, _moduleName);
        }

        protected void rprProducts_ItemCommand(object source, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteItem")
            {
                BuyInTimeService.Delete(SQLDataHelper.GetInt(e.CommandArgument));
            }
        }

        protected string RenderStatus(DateTime start, DateTime expired, bool repeat, int daysRepeat)
        {
            if (start > DateTime.Now)
            {
                return string.Format("<span style='color:black'>{0}</span>", GetLocalResourceObject("BuyInTime_StatusWaiting"));
            }

            else if (repeat || expired > DateTime.Now)
            {
                return string.Format("<span style='color:green'>{0}</span>", GetLocalResourceObject("BuyInTime_StatusStarted"));
            }
            else
            {
                return string.Format("<span style='color:gray'>{0}</span>", GetLocalResourceObject("BuyInTime_StatusComplete"));
            }

        }

    }
}