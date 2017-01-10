using System;
using System.Web;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Mails;
using AdvantShop.Trial;

namespace UserControls.MasterPage
{
    public partial class DemoFeedback : System.Web.UI.UserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Visible = Demo.IsDemoEnabled || TrialService.IsTrialEnabled;
            feedBackCaptcha.Visible = this.Visible;
            liCaptcha.Visible = SettingsMain.EnableCaptcha;
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {

            if (txtSenderName.Text == "" || txtEmail.Text == "" || txtMessage.Text == "" || !feedBackCaptcha.IsValid())
            {
                ((AdvantShopClientPage)Page).ShowMessage(Notify.NotifyType.Error, Resources.Resource.Client_Feedback_WrongData);
                feedBackCaptcha.TryNew();
                return;
            }


            string message = String.Format("Имя:{0} <br/>E-mail:{1}<br/>Сообщение:{2}",
                                           HttpUtility.HtmlEncode(txtSenderName.Text), HttpUtility.HtmlEncode(txtEmail.Text),
                                           HttpUtility.HtmlEncode(txtMessage.Text));

            SendMail.SendMailNow("support@advantshop.net", "Отзыв с сайта " + SettingsMain.SiteUrl, message, true);
            txtSenderName.Text = "";
            txtEmail.Text = "";
            txtMessage.Text = "";

            ((AdvantShopClientPage)Page).ShowMessage(Notify.NotifyType.Notice,Resources.Resource.Client_Feedback_MessageSent);
            feedBackCaptcha.TryNew();
        }

        protected string GetCssClass()
        {
            if (SettingProvider.GetConfigSettingValue<string>("Version").ToLower().Contains("beta"))
            {
                return "link-feedback-beta";
            }
            if (Demo.IsDemoEnabled)
            {
                return "link-feedback-demo-" + SettingsMain.Language;
            }
            if (TrialService.IsTrialEnabled)
            {
                return "link-feedback-trial-" + SettingsMain.Language;
            }
            return "link-feedback-" + SettingsMain.Language;
        }
    }
}