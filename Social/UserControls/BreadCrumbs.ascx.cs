//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using AdvantShop.CMS;

namespace Social.UserControls
{
    public partial class BreadCrumbsUserControl : System.Web.UI.UserControl
    {
        public List<BreadCrumbs> Items = new List<BreadCrumbs>();
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Items.Count == 0)
            {
                Visible = false;
            }

            var result = new StringBuilder();
            result.Append("<div class=\"crumbs\">");


            for (int i = 0; i < Items.Count; ++i)
            {

                if (!string.IsNullOrEmpty(Items[i].Url))
                    result.AppendFormat("<a href=\"{0}\">{1}</a>", Items[i].Url, Items[i].Name);
                else
                    result.Append(Items[i].Name);
                if (i != Items.Count - 1)
                {
                    result.Append("<span class=\"arrow\"></span>");
                }
            }
            result.Append("</div>");
            ltrlNavi.Text = result.ToString();
        }
    }
}