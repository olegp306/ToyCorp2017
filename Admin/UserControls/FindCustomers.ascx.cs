//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using Resources;

namespace Admin.UserControls
{
    public partial class FindCustomers : System.Web.UI.UserControl
    {
        protected SqlPaging _paging;
        protected void Page_Load(object sender, EventArgs e)
        {
            _paging = new SqlPaging {TableName = "[Customers].[Customer]"};

            _paging.AddFieldsRange(new List<Field>
                {
                    new Field {Name = "CustomerID as ID", IsDistinct = true},
                    new Field {Name = "EMail"},
                    new Field {Name = "FirstName"},
                    new Field {Name = "LastName"},
                    new Field {Name = "CustomerRole"},
                });
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            UpdateCustomerNavigatePanel();
        }

        protected void lbNextPage_Click(object sender, EventArgs e)
        {
            var temp = SQLDataHelper.GetInt(currentPage.Value) + 1;
            currentPage.Value = temp.ToString();
            _paging.CurrentPageIndex = temp;
        }

        protected void lbPreviousPage_Click(object sender, EventArgs e)
        {
            var temp = SQLDataHelper.GetInt(currentPage.Value) - 1;
            currentPage.Value = temp.ToString();
            _paging.CurrentPageIndex = temp;
        }

        protected void ddlCurrentPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            _paging.CurrentPageIndex = SQLDataHelper.GetInt(ddlCurrentPage.SelectedValue);
        }
        private void UpdateCustomerNavigatePanel()
        {
            if (CustomerContext.CurrentCustomer.CustomerRole == Role.Moderator)
            {
                _paging.Fields["CustomerRole"].Filter = new EqualFieldFilter() { ParamName = "@customerRole", Value = ((int)Role.User).ToString() };
            }

            rCustomers.DataSource = _paging.PageItems;
            rCustomers.DataBind();

            if (rCustomers.Controls.Count < 3)
            {
                var tr = new TableRow();
                tr.Controls.Add(new TableCell() { Text = Resource.Admin_ViewCustomer_CustomerNotFound });
                rCustomers.Controls.AddAt(1, tr);
            }

            if (_paging.CurrentPageIndex > 1)
            {
                lbPreviousPage.Enabled = true;
            }

            if (_paging.PageCount > 1 && _paging.CurrentPageIndex < _paging.PageCount)
            {
                lbNextPage.Enabled = true;
            }

            ddlCurrentPage.Items.Clear();

            for (int i = 1; i <= _paging.PageCount; i++)
            {
                var itm = new ListItem(i.ToString(), i.ToString());
                if (i == _paging.CurrentPageIndex)
                {
                    itm.Selected = true;
                }
                ddlCurrentPage.Items.Add(itm);
            }
        }
        protected void btnFindCustomer_Click(object sender, EventArgs e)
        {
            _paging.Fields["EMail"].Filter = !string.IsNullOrEmpty(txtSEmail.Text) ? new EqualFieldFilter() { ParamName = "@email", Value = txtSEmail.Text } : null;
            _paging.Fields["FirstName"].Filter = !string.IsNullOrEmpty(txtSFirstName.Text) ? new EqualFieldFilter() { ParamName = "@firstName", Value = txtSFirstName.Text } : null;
            _paging.Fields["LastName"].Filter = !string.IsNullOrEmpty(txtSLastName.Text) ? new EqualFieldFilter() { ParamName = "@lastName", Value = txtSLastName.Text } : null;
            _paging.CurrentPageIndex = 1;
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSEmail.Text = string.Empty;
            txtSFirstName.Text = string.Empty;
            txtSLastName.Text = string.Empty;
            btnFindCustomer_Click(sender, e);
        }

        protected void rCustomers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName.Equals("DeleteCustomer"))
            {
                CustomerService.DeleteCustomer(Guid.Parse((string)e.CommandArgument));
            }
        }
    }
}