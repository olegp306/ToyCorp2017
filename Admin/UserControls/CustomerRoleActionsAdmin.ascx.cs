//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web.UI.WebControls;
using AdvantShop.Core.Caching;
using AdvantShop.Customers;
using AdvantShop.Helpers;

namespace Admin.UserControls
{
    public partial class CustomerRoleActions : System.Web.UI.UserControl
    {
        protected string _category;

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Visible) return;

            _category = "";

            Guid customerId = new Guid(Request["customerid"]);
            var roleActionList = RoleActionService.GetRoleActions();
            var customerRoleKeysList = RoleActionService.GetCustomerRoleKeysByCustomerId(customerId);

            foreach (RoleActionKey key in customerRoleKeysList)
            {
                var role = roleActionList.Find(r => r.Key == key);
                if (role != null)
                    role.Enabled = true;
            }

            rprAccessSettigs.DataSource = roleActionList;
            rprAccessSettigs.DataBind();

        }

        public void SaveRole()
        {
            Guid customerId = new Guid(Request["customerid"]);

            foreach (RepeaterItem item in rprAccessSettigs.Items)
            {
                string roleActionKey = ((HiddenField)item.FindControl("hfRoleActionKey")).Value;
                bool enabled = SQLDataHelper.GetBoolean(((CheckBox)item.FindControl("chkRoleAction")).Checked);

                RoleActionService.UpdateOrInsertCustomerRoleAction(customerId, roleActionKey, enabled);
            }

            var cacheName = CacheNames.GetRoleActionsCacheObjectName(customerId.ToString());
            if (CacheManager.Contains(cacheName))
            {
                CacheManager.Remove(cacheName);
            }
        }

        protected string RenderCategory(string category)
        {
            if (category != _category)
            {
                _category = category;
                return String.Format("<tr><td colspan='2' style=\"font-weight: bold; padding:15px 0 5px 0;\">{0}</td></tr>", category);
            }

            return "";
        }
    }
}