//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web.UI;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Trial;

namespace UserControls.Default
{
    public partial class Carousel : UserControl
    {

        protected int CarouselsCount { get; set; }

        public string CssSlider { get; set; }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            CarouselsCount = CarouselService.GetCarouselsCount();
            if ((CarouselsCount == 0 && !AdvantShop.Customers.CustomerContext.CurrentCustomer.IsAdmin && !TrialService.IsTrialEnabled) || SettingsDesign.CarouselVisibility == false)
            {
                this.Visible = false;
            }
        }
    }
}