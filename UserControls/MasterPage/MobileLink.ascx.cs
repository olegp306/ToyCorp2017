using System;
using AdvantShop.Helpers;

namespace UserControls.MasterPage
{
    public partial class MobileLink : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Visible = MobileHelper.IsMobileBrowser() && MobileHelper.ExistMobileTemplate();
        }

        protected void toMobileLink_Click(object sender, EventArgs e)
        {
            MobileHelper.DeleteDesktopForcedCookie();
            MobileHelper.RedirectToMobile(Context);
        }

    }
}