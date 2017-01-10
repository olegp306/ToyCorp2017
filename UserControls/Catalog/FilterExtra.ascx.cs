using System;
using System.Web.UI;
using AdvantShop.Configuration;

//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace UserControls.Catalog
{
    public partial class UserControls_FilterExtra : UserControl
    {
        public bool AvailableSelected { get; set; }
        public bool PreOrderSelected { get; set; }
        public int CategoryId { get; set; }
        public bool InDepth { get; set; }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            divAvaliable.Visible = SettingsCatalog.AvaliableFilterEnabled;
            divPreorder.Visible = SettingsCatalog.PreorderFilterEnabled;

            this.Visible = divAvaliable.Visible || divPreorder.Visible;
        }
    }
}