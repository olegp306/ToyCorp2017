using System;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using Resources;

namespace UserControls.MyAccount
{
    public partial class CommonInformation : System.Web.UI.UserControl
    {
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (SettingsMain.EnablePhoneMask)
            {
                txtContacts.CssClass = "mask-phone mask-inp";
            }

            Customer customer = CustomerContext.CurrentCustomer;
            lblEmail.Text = customer.EMail;
            lblRegistrationDate.Text = AdvantShop.Localization.Culture.ConvertDate(customer.RegistrationDateTime);
            txtFirstName.Text = HttpUtility.HtmlDecode(customer.FirstName);
            txtLastName.Text = HttpUtility.HtmlDecode(customer.LastName);
            txtContacts.Text = HttpUtility.HtmlDecode(customer.Phone);

            newsSubscription.Visible = SettingsDesign.NewsSubscriptionVisibility;
            chkSubscribed4News.Checked = SubscriptionService.IsSubscribe(customer.EMail);

            if (CustomerGroupService.DefaultCustomerGroup == customer.CustomerGroup.CustomerGroupId)
            {
                liCustomerGroup.Visible = false;
            }
            else
            {
                lCustomerGroup.Text = customer.CustomerGroup.GroupName;
            }
            switch (customer.CustomerRole)
            {
                case Role.User:
                    lCustomerType.Text = Resource.Client_MyAccount_User;
                    liCustomerRole.Visible = false;
                    break;
                case Role.Moderator:
                    lCustomerType.Text = Resource.Client_MyAccount_Moderator;
                    break;
                case Role.Administrator:
                    lCustomerType.Text = Resource.Client_MyAccount_Administrator;
                    break;
            }
        }

        protected void llbChangePassword_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.Url.AbsolutePath + QueryHelper.ChangeQueryParam(Request.Url.Query, "View", "ChangePassword"));
        }

        private bool ValidateData()
        {
            bool valid = true;

            valid &= !string.IsNullOrEmpty(txtFirstName.Text);
            valid &= !string.IsNullOrEmpty(txtLastName.Text);
            return valid;
        }

        protected void btnChangeCommonInfo_Click(object sender, EventArgs e)
        {
            if (!ValidateData())
                return;
            var customer = CustomerContext.CurrentCustomer;
            customer.FirstName = HttpUtility.HtmlEncode(txtFirstName.Text);
            customer.LastName = HttpUtility.HtmlEncode(txtLastName.Text);
            customer.Phone = HttpUtility.HtmlEncode(txtContacts.Text);
            customer.SubscribedForNews = chkSubscribed4News.Checked;

            if (CustomerService.UpdateCustomer(customer))
            {
                ((AdvantShopClientPage)this.Page).ShowMessage(Notify.NotifyType.Notice, Resource.Client_MyAccount_DataSuccessSaved);
            }
            else
            {
                ((AdvantShopClientPage)this.Page).ShowMessage(Notify.NotifyType.Error, Resource.Client_MyAccount_ErrorSavingData);
            }
        }
    }
}
