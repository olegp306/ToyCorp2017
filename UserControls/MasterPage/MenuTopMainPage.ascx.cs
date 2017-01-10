using System;

namespace UserControls.MasterPage
{
    public partial class MenuTopMainPage : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            menuCatalogAlternative.Visible = !Request.Url.AbsolutePath.ToLower().Contains("default.aspx");
        }
    }
}