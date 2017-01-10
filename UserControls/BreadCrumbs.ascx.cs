using System;
using System.Collections.Generic;
using AdvantShop.CMS;
using System.Text;

namespace UserControls
{
    public partial class BreadCrumbsControl : System.Web.UI.UserControl
    {
        public List<BreadCrumbs> Items = new List<BreadCrumbs>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Items.Count == 0)
            {
                this.Visible = false;
            }

            var result = new StringBuilder();
            result.Append("<div class=\"crumbs\">");

            for (int i = 0; i < Items.Count; ++i)
            {
                if (!string.IsNullOrEmpty(Items[i].Url) && i != Items.Count - 1)
                {
                    result.AppendFormat("<a href=\"{0}\">{1}</a>", Items[i].Url, Items[i].Name);
                }
                else
                {
                    //id for inplaceEditor
                    result.AppendFormat("<span data-inplace-update=\"crumbs\">{0}</span>", Items[i].Name);
                }
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