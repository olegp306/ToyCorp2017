using System;
using System.Linq;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Globalization;

using AdvantShop.Customers;
using AdvantShop.Modules;

namespace Advantshop.UserControls.Modules
{
    public partial class Admin_UniSenderSettings : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void Save()
        {
            var lastUniSenderId = UniSenderSettings.ApiKey;
            UniSenderSettings.FromName = txtFromName.Text;
            UniSenderSettings.FromEmail = txtFromEmail.Text;
            UniSenderSettings.ApiKey = txtUniSenderId.Text;

            if (string.IsNullOrEmpty(txtUniSenderId.Text))
            {
                UniSenderService.UnsubscribeListMembers(UniSenderSettings.RegUsersList);
                UniSenderService.UnsubscribeListMembers(UniSenderSettings.OrderCustomersList);
                return;
            }

            var currentUnisenderId = UniSenderSettings.ApiKey;

            if (!ValidateData())
            {
                lblMessage.Text = (string)GetLocalResourceObject("UniSender_Error");
                lblMessage.ForeColor = Color.Red;

                UniSenderSettings.ApiKey = lastUniSenderId;
                return;
            }

            if (!string.Equals(lastUniSenderId, currentUnisenderId))
            {
                UniSenderSettings.RegUsersList = string.Empty;
                UniSenderSettings.OrderCustomersList = string.Empty;
                return;
            }

            if (ddlUniSenderListsReg.SelectedValue == "0")
            {
                UniSenderService.UnsubscribeListMembers(UniSenderSettings.RegUsersList);
            }
            else
            {
                UniSenderService.SubscribeListMembers(ddlUniSenderListsReg.SelectedValue,
                    SubscriptionService.GetSubscribedEmails());
            }

            if (ddlUniSenderListsOrderCustomers.SelectedValue == "0")
            {
                UniSenderService.UnsubscribeListMembers(UniSenderSettings.OrderCustomersList);
            }
            else
            {
                UniSenderService.SubscribeListMembers(ddlUniSenderListsOrderCustomers.SelectedValue,
                    AdvantShop.Orders.OrderService.GetOrderCustomersEmails());
            }

            UniSenderSettings.RegUsersList = ddlUniSenderListsReg.SelectedValue != "0" 
                ? ddlUniSenderListsReg.SelectedValue : string.Empty;
            UniSenderSettings.OrderCustomersList = ddlUniSenderListsOrderCustomers.SelectedValue != "0"
                ? ddlUniSenderListsOrderCustomers.SelectedValue : string.Empty;

            LoadData();
            lblMessage.Text = (string)GetLocalResourceObject("UniSender_ChangesSaved");
            lblMessage.ForeColor = Color.Blue;
            lblMessage.Visible = true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        protected void LoadData()
        {
            txtUniSenderId.Text = UniSenderSettings.ApiKey;
            txtFromName.Text = UniSenderSettings.FromName;
            txtFromEmail.Text = UniSenderSettings.FromEmail;

            var lists = UniSenderService.GetLists();
            if (lists != null && lists.Count > 0)
            {
                ddlUniSenderListsReg.DataSource = lists;
                ddlUniSenderListsReg.DataBind();
                if (lists.All(item => item.id != UniSenderSettings.RegUsersList))
                    UniSenderSettings.RegUsersList = string.Empty;
                ddlUniSenderListsReg.SelectedValue = string.IsNullOrEmpty(UniSenderSettings.RegUsersList)
                    ? "0"
                    : UniSenderSettings.RegUsersList;

                ddlUniSenderListsOrderCustomers.DataSource = lists;
                ddlUniSenderListsOrderCustomers.DataBind();
                if (lists.All(item => item.id != UniSenderSettings.OrderCustomersList))
                    UniSenderSettings.OrderCustomersList = string.Empty;
                ddlUniSenderListsOrderCustomers.SelectedValue = string.IsNullOrEmpty(UniSenderSettings.OrderCustomersList)
                    ? "0"
                    : UniSenderSettings.OrderCustomersList;
            }
            else
            {
                ddlUniSenderListsReg.Items.Clear();
                ddlUniSenderListsReg.Items.Add(new ListItem
                    {
                        Text = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ru" ? "Нет привязки к списку" : "No binding to the list",
                        Value = @"0"
                    });
                ddlUniSenderListsReg.DataBind();

                ddlUniSenderListsOrderCustomers.Items.Clear();
                ddlUniSenderListsOrderCustomers.Items.Add(new ListItem
                    {
                        Text = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ru" ? "Нет привязки к списку" : "No binding to the list",
                        Value = @"0"
                    });
                ddlUniSenderListsOrderCustomers.DataBind();
            }
        }

        private bool ValidateData()
        {
            var valid = true;

            valid &= !string.IsNullOrEmpty(txtUniSenderId.Text);

            if (valid)
            {
                valid &= !string.IsNullOrEmpty(txtFromName.Text) && !string.IsNullOrEmpty(txtFromEmail.Text);
                valid &= AdvantShop.Helpers.ValidationHelper.IsValidEmail(txtFromEmail.Text);
                valid &= !txtFromName.Text.Contains("\"");
            }
            return valid;
        }
    }
}