//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.BonusSystem;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Security;
using AdvantShop.Trial;
using Resources;

namespace Admin
{
    public partial class ViewCustomer : AdvantShopAdminPage
    {
        protected Customer customer;
        protected bool ShowRoleAccess = false;

        private void MsgErr(bool clean)
        {
            if (clean)
            {
                Message.Visible = false;
                Message.Text = "";
            }
            else
            {
                Message.Visible = false;
            }
        }

        private void MsgErr(string messageText)
        {
            Message.Visible = true;
            Message.Text = @"<br/>" + messageText;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_ViewCustomer_Header));
            MsgErr(true);
            if (string.IsNullOrEmpty(Request["customerid"]))
            {
                Response.Redirect("default.aspx");
            }
            else
            {
                customer = CustomerService.GetCustomer(Request["customerid"].TryParseGuid());
            }

            CustomerAddressBookAdmin.Customer = customer;

            if (IsPostBack) return;

            ddlCustomerRole.Items.Add(new ListItem(Resource.Admin_ViewCustomer_CustomerRole_User, ((int)Role.User).ToString()));
            ddlCustomerRole.Items.Add(new ListItem(Resource.Admin_ViewCustomer_CustomerRole_Moderator, ((int)Role.Moderator).ToString()));
            if (CustomerContext.CurrentCustomer.IsAdmin || TrialService.IsTrialEnabled)
                ddlCustomerRole.Items.Add(new ListItem(Resource.Admin_ViewCustomer_CustomerRole_Administrator, ((int)Role.Administrator).ToString()));

            var currentCustomer = CustomerContext.CurrentCustomer;
            lblCustomerName.Text = customer.LastName + @" " + customer.FirstName;

            //lblLogin.Text = Customer.Login;
            lblRegistrationDate.Text = AdvantShop.Localization.Culture.ConvertDate(customer.RegistrationDateTime);
            txtFirstName.Text = customer.FirstName;
            txtLastName.Text = customer.LastName;
            txtWWW.Text = customer.Phone;
            txtEmail.Text = customer.EMail;
            chkSubscribed4News.Checked = SubscriptionService.IsSubscribe(customer.EMail);

            ddlCustomerRole.SelectedValue = ((int)customer.CustomerRole).ToString();
            ddlCustomerRole.Visible = true;
            lRole.Visible = false;

            foreach (var group in CustomerGroupService.GetCustomerGroupList())
            {
                ddlCustomerGroup.Items.Add(new ListItem(string.Format("{0} - {1}%", group.GroupName, group.GroupDiscount), group.CustomerGroupId.ToString()));
            }
            ddlCustomerGroup.SelectedValue = customer.CustomerGroupId.ToString();

            ShowRoleAccess = customer.CustomerRole == Role.Moderator && currentCustomer.CustomerRole == Role.Administrator;
            CustomerRoleActionsAdmin.Visible = ShowRoleAccess;

            trCustomerRole.Visible = currentCustomer.CustomerRole == Role.Administrator || TrialService.IsTrialEnabled;
            llbChangePassword.Visible = currentCustomer.CustomerRole != Role.Moderator || (currentCustomer.CustomerRole == Role.Moderator && customer.CustomerRole != Role.Administrator);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

            if (BonusSystem.IsActive)
            {
                divBonusCard.Visible = true;
                trBonusAmount.Visible = false;

                var card = BonusSystemService.GetCard(customer.BonusCardNumber);
                if (card != null)
                {
                    txtBonusCardNumber.Text = card.CardNumber.ToString();
                    lblBonusCardAmount.Text = card.BonusAmount.ToString();
                    trBonusAmount.Visible = true;
                }
            }
        }

        protected void btnChangeCommonInfo_Click(object sender, EventArgs e)
        {
            int groupId;
            Int32.TryParse(ddlCustomerGroup.SelectedValue, out groupId);
            lblError.Visible = false;
            customer.FirstName = HttpUtility.HtmlEncode(txtFirstName.Text);
            customer.LastName = txtLastName.Text;
            customer.Phone = txtWWW.Text;
            customer.CustomerGroupId = groupId;
            customer.SubscribedForNews = chkSubscribed4News.Checked;

            if (customer.EMail != txtEmail.Text && CustomerService.ExistsEmail(txtEmail.Text))
            {
                lblError.Text = Resource.Admin_CreateCustomer_CustomerErrorEmailExist;
                lblError.Visible = true;
                return;
            }

            customer.EMail = txtEmail.Text; customer.EMail = txtEmail.Text;

            var cardNumber = txtBonusCardNumber.Text.TryParseLong(true);
            if (BonusSystem.IsActive && txtBonusCardNumber.Text.IsNotEmpty())
            {
                if (BonusSystemService.GetCard(cardNumber) == null)
                {
                    lblError.Text = Resource.Admin_ViewCustomer_WrongCardNumber;
                    lblError.Visible = true;
                    return;
                }
            }

            customer.BonusCardNumber = cardNumber;
            
            var prevCustomerRole = customer.CustomerRole;
            customer.CustomerRole = (Role)SQLDataHelper.GetInt(ddlCustomerRole.SelectedValue);

            if (customer.CustomerRole == Role.Moderator)
            {
                CustomerRoleActionsAdmin.SaveRole();
            }
            else if (prevCustomerRole == Role.Moderator && (customer.CustomerRole != Role.Moderator))
            {
                RoleActionService.DeleteCustomerRoleActions(customer.Id);
            }
            
            CustomerService.UpdateCustomer(customer);
            ShowRoleAccess = customer.CustomerRole == Role.Moderator;
            CustomerRoleActionsAdmin.Visible = ShowRoleAccess;
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            ulUserRegistarionValidation.InnerHtml = "";
            bool boolIsValidPast = true;

            if (!string.IsNullOrEmpty(txtNewPassword.Text.Trim()) && txtNewPassword.Text.Length >= 6)
            {
                txtNewPassword.CssClass = "OrderConfirmation_ValidTextBox";
            }
            else
            {
                txtNewPassword.CssClass = "OrderConfirmation_InvalidTextBox";
                ulUserRegistarionValidation.InnerHtml += string.Format("<li>{0}</li>", Resource.Client_Registration_PasswordLenght);
                boolIsValidPast = false;
            }

            if (string.IsNullOrEmpty(txtNewPasswordConfirm.Text.Trim()) == false)
            {
                txtNewPasswordConfirm.CssClass = "OrderConfirmation_ValidTextBox";
            }
            else
            {
                txtNewPasswordConfirm.CssClass = "OrderConfirmation_InvalidTextBox";
                boolIsValidPast = false;
            }

            if ((string.IsNullOrEmpty(txtNewPasswordConfirm.Text) == false) &&
                (string.IsNullOrEmpty(txtNewPassword.Text) == false) && (txtNewPassword.Text == txtNewPasswordConfirm.Text))
            {
                txtNewPassword.CssClass = "OrderConfirmation_ValidTextBox";
                txtNewPasswordConfirm.CssClass = "OrderConfirmation_ValidTextBox";
            }
            else
            {
                txtNewPassword.CssClass = "OrderConfirmation_InvalidTextBox";
                txtNewPasswordConfirm.CssClass = "OrderConfirmation_InvalidTextBox";
                ulUserRegistarionValidation.InnerHtml += string.Format("<li>{0}</li>", Resource.Client_Registration_PasswordNotMatch);
                boolIsValidPast = false;
            }

            if (boolIsValidPast)
            {
                CustomerService.ChangePassword(customer.Id, txtNewPassword.Text, false);
                if (CustomerContext.CurrentCustomer.Id == customer.Id)
                {
                    AdvantShop.Security.AuthorizeService.SignIn(customer.EMail, txtNewPassword.Text, false, true);
                }
                MsgErr(Resource.Admin_ViewCustomer_PasswordSaved);
                mvCommonInfo.SetActiveView(vCommonInfo);
            }
            else
            {
                ulUserRegistarionValidation.Visible = true;
                MsgErr(Resource.Admin_ViewCustomer_DiffrentPasswords);
            }
        }

        protected void llbChangePassword_Click(object sender, EventArgs e)
        {
            mvCommonInfo.SetActiveView(vChangePassword);
        }
    }
}