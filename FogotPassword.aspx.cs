//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Helpers;
using AdvantShop.Customers;
using AdvantShop.Mails;
using AdvantShop.SEO;
using AdvantShop.Security;
using Resources;

namespace ClientPages
{
    public partial class FogotPassword : AdvantShopClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(
                new MetaInfo(string.Format("{0} - {1}", SettingsMain.ShopName,
                                           Resource.Client_FogotPassword_PasswordRecovery)), null);

            if (!string.IsNullOrEmpty(Request["email"]) && !string.IsNullOrEmpty(Request["recoverycode"]))
            {
                var customer = CustomerService.GetCustomerByEmail(Request["email"]);
                if (customer == null) return;

                if (ValidationHelper.DeleteSigns(SecurityHelper.GetPasswordHash(customer.Password)).ToLower() !=
                    Request["recoverycode"].ToLower())
                {
                    MultiView1.SetActiveView(ViewRecoveryError);
                }
                else
                {
                    MultiView1.SetActiveView(ViewRecovery);
                }
            }
            else
            {
                MultiView1.SetActiveView(ViewDataCollecting);
            }
        }

        protected void btnSendPasswordByEmail_Click(object sender, EventArgs e)
        {
            var customer = CustomerService.GetCustomerByEmail(txtEmail.Text);

            if (customer == null)
            {
                MultiView1.SetActiveView(ViewEmailSendError);
                return;
            }

            string strLink = SettingsMain.SiteUrl + "/FogotPassword.aspx?Email=" + customer.EMail + "&RecoveryCode=" +
                             ValidationHelper.DeleteSigns(SecurityHelper.GetPasswordHash(customer.Password));

            var pwdRepairMail = new PwdRepairMailTemplate(ValidationHelper.DeleteSigns(SecurityHelper.GetPasswordHash(customer.Password)).ToLower(), 
                                                            customer.EMail, strLink);
            pwdRepairMail.BuildMail();

            SendMail.SendMailNow(customer.EMail, pwdRepairMail.Subject, pwdRepairMail.Body, true);

            MultiView1.ActiveViewIndex = 1;
        }


        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (txtNewPassword.Text == txtNewPasswordConfirm.Text)
            {
                var customer = CustomerService.GetCustomerByEmail(Request["email"]);
                if (customer == null) return;

                if (ValidationHelper.DeleteSigns(SecurityHelper.GetPasswordHash(customer.Password)).ToLower() ==
                    Request["recoverycode"].ToLower())
                {
                    CustomerService.ChangePassword(customer.Id, txtNewPassword.Text, false);
                    AuthorizeService.SignIn(Request["email"], txtNewPassword.Text, false, true);
                    MultiView1.SetActiveView(ViewPasswordChanged);
                }
                else
                {
                    MultiView1.SetActiveView(ViewRecoveryError);
                }
            }
            else
            {
                ShowMessage(Notify.NotifyType.Notice, Resources.Resource.Client_FogotPassword_PasswordDiffrent);
            }
        }

    }
}