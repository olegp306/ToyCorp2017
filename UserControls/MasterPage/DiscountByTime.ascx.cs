using System;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Helpers;

namespace UserControls.MasterPage
{
    public partial class DiscountByTimeControl : System.Web.UI.UserControl
    {
        protected string popupText;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (DiscountByTimeService.ShowPopup && CommonHelper.GetCookieString("discountbytime").IsNullOrEmpty()
                && DiscountByTimeService.GetDiscountByTime() != 0)
            {
                popupText = DiscountByTimeService.PopupText;
                CommonHelper.SetCookie("discountbytime", "true", new TimeSpan(12, 0, 0), true);
            }
            else
            {
                this.Visible = false;
            }
        }
    }
}