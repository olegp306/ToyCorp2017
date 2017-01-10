using System;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Customers;
using AdvantShop.Security;
using Resources;

namespace UserControls.MyAccount
{
    public partial class ChangePassword : System.Web.UI.UserControl
    {
        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (ValidateFormData())
            {
                CustomerService.ChangePassword(CustomerContext.CurrentCustomer.Id, txtNewPassword.Text, false);
                AuthorizeService.SignIn(CustomerContext.CurrentCustomer.EMail, txtNewPassword.Text, false, true);
                ((AdvantShopClientPage)this.Page).ShowMessage(Notify.NotifyType.Notice, Resource.Client_MyAccount_PasswordSaved);
                txtNewPassword.Text = string.Empty;
                txtNewPasswordConfirm.Text = string.Empty;
            }
        }
        
        private bool ValidateFormData()
        {
            if ( txtNewPassword.Text.Length < 6)
            {
                txtNewPassword.Text = string.Empty;
                txtNewPasswordConfirm.Text = string.Empty;
                ((AdvantShopClientPage)this.Page).ShowMessage(Notify.NotifyType.Error, Resource.Client_Registration_PasswordLenght);
                return false;
            }
            if ((txtNewPasswordConfirm.Text.IsNotEmpty()) && (txtNewPassword.Text.IsNotEmpty()) && (txtNewPassword.Text != txtNewPasswordConfirm.Text))
            {
                txtNewPassword.Text = string.Empty;
                txtNewPasswordConfirm.Text = string.Empty;
                ((AdvantShopClientPage)this.Page).ShowMessage(Notify.NotifyType.Error, Resource.Client_Registration_PasswordNotMatch);
                return false;
            }
            return true;
        }
    }
}