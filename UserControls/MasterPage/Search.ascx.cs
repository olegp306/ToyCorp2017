using System;

namespace UserControls.MasterPage
{
    public partial class Search : System.Web.UI.UserControl
    {
        protected void btnGoSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
            {
                Page.Response.Redirect(string.Format("~/search.aspx?name={0}", txtSearch.Text.Trim()));
            }
        }
    }
}