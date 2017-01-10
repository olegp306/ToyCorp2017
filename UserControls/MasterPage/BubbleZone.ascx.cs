using System;
using AdvantShop.Repository;

namespace UserControls.MasterPage
{
    public partial class BubbleZone : System.Web.UI.UserControl
    {
        protected string CurrentTown = IpZoneContext.CurrentZone.City;

        protected void Page_Load(object sender, EventArgs e)
        {
            Visible = IpZoneContext.ShowBubble();
        }
    }
}