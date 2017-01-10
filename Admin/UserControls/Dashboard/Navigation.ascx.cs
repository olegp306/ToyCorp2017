using System;
using System.Linq;
using System.Web.UI;
using AdvantShop.Customers;

namespace Admin.UserControls.Dashboard
{
    public partial class Navigation : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var _customer = CustomerContext.CurrentCustomer;

            if (_customer.CustomerRole == Role.Moderator)
            {
                var actions = RoleActionService.GetCustomerRoleActionsByCustomerId(_customer.Id);

                dashCatalog.Visible = actions.Any(a => a.Key == RoleActionKey.DisplayCatalog && a.Enabled);
                dashOrder.Visible = actions.Any(a => a.Key == RoleActionKey.DisplayOrders && a.Enabled);
                dashNews.Visible = actions.Any(a => a.Key == RoleActionKey.DisplayNews && a.Enabled);
                dashImportCsv.Visible = actions.Any(a => a.Key == RoleActionKey.DisplayImportExport && a.Enabled);
                dashModules.Visible = actions.Any(a => a.Key == RoleActionKey.DisplayModules && a.Enabled);
                dashDesign.Visible = actions.Any(a => a.Key == RoleActionKey.DisplayDesignTransformer && a.Enabled);

                this.Visible = dashCatalog.Visible || dashOrder.Visible || dashNews.Visible || dashImportCsv.Visible || dashModules.Visible || dashDesign.Visible;
            }
        }
    }
}