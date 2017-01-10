//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web;
using AdvantShop;

namespace Admin.UserControls.MasterPage
{
    public partial class AdminSearch : System.Web.UI.UserControl
    {
        protected string searchRequest = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["search"].IsNotEmpty())
            {
                searchRequest = HttpUtility.HtmlEncode(Request["search"]);
            }

        }
    }
}