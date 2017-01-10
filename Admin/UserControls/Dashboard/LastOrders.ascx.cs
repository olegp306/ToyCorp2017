using System;
using System.Web.UI.WebControls;
using AdvantShop.Orders;

namespace Admin.UserControls.Dashboard
{
    public partial class Admin_UserControls_LastOrders : System.Web.UI.UserControl
    {
        protected void Page_PreRender(object sender, EventArgs e)
        {
            lvLastOrders.DataSource = OrderService.GetLastOrders(7);
            lvLastOrders.DataBind();
        }

        protected void lvLastOrders_ItemCommand(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "DeleteOrder")
            {
                OrderService.DeleteOrder(int.Parse(e.CommandArgument.ToString()));
            }
        }
    }
}