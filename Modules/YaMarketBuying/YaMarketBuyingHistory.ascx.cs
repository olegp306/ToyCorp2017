using System;
using AdvantShop.Modules;

namespace Advantshop.Modules.YaMarketBuyingModuleSetting
{
    public partial class Admin_YaMarketBuyingHistory : System.Web.UI.UserControl
    {
        protected void Page_PreRender(object sender, EventArgs e)
        {
            lvHistory.DataSource = YaMarketByuingService.GetHistory();
            lvHistory.DataBind();
        }
    }
}