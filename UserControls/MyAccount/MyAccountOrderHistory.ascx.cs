using System;

namespace UserControls.MyAccount
{
    public partial class MyAccountOrderHistory : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Visible = String.IsNullOrEmpty(Request["orderid"]);

            if (Visible == false)
                return;
        }
    }
}