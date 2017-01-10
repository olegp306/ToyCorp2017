using System;
using AdvantShop.Trial;

namespace UserControls.MasterPage
{
    public partial class TrialCounters : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Visible = TrialService.IsTrialEnabled;
        }
    }
}