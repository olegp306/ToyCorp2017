using System;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Customers;
using AdvantShop.SEO;
using AdvantShop.Security;

namespace ClientPages
{
    public partial class Login : AdvantShopClientPage
    {
        protected bool DisplayCaptcha {
            get { return CaptchaService.IsIPinList(Request.UserHostAddress); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (CustomerContext.CurrentCustomer.RegistredUser)
            {
                Response.Redirect("~/");
            }
            SetMeta(
                new MetaInfo(string.Format("{0} - {1}", SettingsMain.ShopName, Resources.Resource.Client_Login_Header)),
                string.Empty);

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (DisplayCaptcha && !dnfValid.IsValid())
            {
                ShowMessage(Notify.NotifyType.Error, Resources.Resource.Client_Feedback_WrongData);
                dnfValid.TryNew();
                return;
            }

            if (!(string.IsNullOrEmpty(txtEmail.Text.Trim()) || string.IsNullOrEmpty(txtPassword.Text.Trim())))
            {
                if (!AuthorizeService.SignIn(txtEmail.Text, txtPassword.Text, false, true))
                {
                    CaptchaService.AddIPtoList(Request.UserHostAddress);
                    dnfValid.TryNew();
                    ShowMessage(Notify.NotifyType.Error, Resources.Resource.Client_MasterPage_WrongPassword);
                }
                else
                {
                    CaptchaService.RemoveIPfromList(Request.UserHostAddress);
                    Response.Redirect("~/");
                }
                
            }
        }
    }
}