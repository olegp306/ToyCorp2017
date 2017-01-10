//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Customers;

namespace UserControls.Catalog
{
    public partial class RecentlyView : System.Web.UI.UserControl
    {
        public int ProductsToShow { set; get; }

        public RecentlyView()
        {
            ProductsToShow = 3;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Browser.Crawler || !SettingsDesign.RecentlyViewVisibility)
            {
                this.Visible = false;
                return;
            }

            var tempList = RecentlyViewService.LoadViewDataByCustomer(CustomerContext.CustomerId, ProductsToShow);
            if (tempList.Any())
            {
                lvRecentlyView.DataSource = tempList;
                lvRecentlyView.DataBind();
            }
            else
            {
                Visible = false;
            }
        }
    }
}