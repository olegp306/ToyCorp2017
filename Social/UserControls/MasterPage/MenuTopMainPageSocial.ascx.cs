using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UserControls_MasterPage_MenuTopMainPageSocial : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        menuCatalogAlternative.Visible = !Request.Url.AbsolutePath.ToLower().Contains("default.aspx");
    }
}