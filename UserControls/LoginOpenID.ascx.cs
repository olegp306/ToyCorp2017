using System;
using System.Web.UI;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Helpers;
using AdvantShop.Security.OpenAuth;

namespace UserControls
{
    public partial class LoginOpenID : UserControl
    {
        public string PageToRedirect = string.Empty;
              

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!(SettingsOAuth.GoogleActive || SettingsOAuth.YandexActive || SettingsOAuth.TwitterActive ||
                  SettingsOAuth.VkontakteActive || SettingsOAuth.FacebookActive || SettingsOAuth.MailActive ||
                  SettingsOAuth.OdnoklassnikiActive))
            {
                Visible = false;
                return;
            }

            var rootUrlPath = Request.Url.AbsoluteUri.Contains("localhost") ? "~/" : StringHelper.MakeASCIIUrl(SettingsMain.SiteUrl);
            var strRedirectUrl = PageToRedirect.IsNotEmpty() ? rootUrlPath + "/" + PageToRedirect : rootUrlPath;
            
            if (SettingsOAuth.VkontakteActive && !string.IsNullOrEmpty(Request["code"]) && string.Equals(Request["auth"], "vk"))
            {
                VkontakteOauth.VkontakteAuth(Request["code"], string.Empty, PageToRedirect);
                Response.Redirect(strRedirectUrl, false);
            }

            if (SettingsOAuth.OdnoklassnikiActive && !string.IsNullOrEmpty(Request["code"]) && string.Equals(Request["auth"], "od"))
            {
                OdnoklassnikiOauth.OdnoklassnikiLogin(Request["code"]);
                Response.Redirect(strRedirectUrl, false);
            }

            if (SettingsOAuth.GoogleActive && !string.IsNullOrEmpty(Request["code"]) && string.Equals(Request["auth"], "google"))
            {
                GoogleOauth.ExchangeCodeForAccessToken(Request["code"]);
                Response.Redirect(strRedirectUrl, false);
            }
            
            if (SettingsOAuth.FacebookActive && !string.IsNullOrEmpty(Request["code"]))
            {
                FacebookOauth.SendFacebookRequest(Request["code"], SettingsMain.SiteUrl + "/" + PageToRedirect);
                Response.Redirect(strRedirectUrl, false);
            }

            if ((SettingsOAuth.GoogleActive || SettingsOAuth.YandexActive || SettingsOAuth.MailActive)
              && OAuthResponce.OAuthUser(Request))
            {
                Response.Redirect(strRedirectUrl, false);
            }
        }

        protected void lnkbtnVkClick(object sender, EventArgs e)
        {
            VkontakteOauth.VkontakteAuthDialog(PageToRedirect);
        }

        protected void lnkbtnFacebookClick(object sender, EventArgs e)
        {
            FacebookOauth.ShowAuthDialog(SettingsMain.SiteUrl + "/" + PageToRedirect);
        }

        protected void lnkbtnMailClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtOauthUserId.Text))
                return;
            var userId = txtOauthUserId.Text;
            
            var userIdAndDomainPair = txtOauthUserId.Text.Split(new[] { '@' });
            if (userIdAndDomainPair.Length != 2)
            {
                return;
            }

            var oAuthRequest = new OAuthRequest { UserId = userId, Provider = OAuthRequest.Providers.Mail };
            oAuthRequest.CreateRequest(new ClaimParameters(), true);
        }

        //protected void lnkbtnTwitterClick(object sender, EventArgs e)
        //{
        //    TwitterOAuth.TwitterOpenAuth();
        //}

        protected void lnkbtnGoogleClick(object sender, EventArgs e)
        {
            GoogleOauth.SendAuthenticationRequest();
        }

        protected void lnkbtnYandexClick(object sender, EventArgs e)
        {
            var oAuthRequest = new OAuthRequest { Provider = OAuthRequest.Providers.Yandex };
            oAuthRequest.CreateRequest(new ClaimParameters(), false);
        }

        protected void lnkbtnOdnoklassnikiClick(object sender, EventArgs e)
        {
            OdnoklassnikiOauth.OdnoklassnikiAuthDialog();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            lnkbtnGoogle.Visible = SettingsOAuth.GoogleActive;
            lnkbtnYandex.Visible = SettingsOAuth.YandexActive;
            //lnkbtnTwitter.Visible = SettingsOAuth.TwitterActive;
            lnkbtnVk.Visible = SettingsOAuth.VkontakteActive;
            lnkbtnFacebook.Visible = SettingsOAuth.FacebookActive;
            lnkbtnMail.Visible = pnlMail.Visible = SettingsOAuth.MailActive;
            lnkbtnOdnoklassniki.Visible = SettingsOAuth.OdnoklassnikiActive;
        }
    }
}