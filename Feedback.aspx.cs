//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Mails;
using AdvantShop.Controls;
using AdvantShop;
using AdvantShop.SEO;
using Resources;

namespace ClientPages
{
    public partial class Feedback : AdvantShopClientPage
    {
        protected Customer curentCustomer = CustomerContext.CurrentCustomer;

        protected void btnSend_Click(object sender, EventArgs e)
        {
            bool boolIsValidPast = true;

            boolIsValidPast = txtSenderName.Text.IsNotEmpty() && txtMessage.Text.IsNotEmpty() &&
                              txtEmail.Text.IsNotEmpty() &&
                              AdvantShop.Helpers.ValidationHelper.IsValidEmail(txtEmail.Text);

            if (!boolIsValidPast)
            {
                ShowMessage(Notify.NotifyType.Error, Resource.Client_Feedback_WrongData);
                validShield.TryNew();
                return;
            }

            if (SettingsMain.EnableCaptcha && !validShield.IsValid())
            {
                ShowMessage(Notify.NotifyType.Error, Resource.Client_Feedback_WrongCaptcha);
                validShield.TryNew();
                return;
            }

            try
            {
                var mailTemplate = new FeedbackMailTemplate(SettingsMain.SiteUrl, SettingsMain.ShopName,
                                                            HttpUtility.HtmlEncode(txtSenderName.Text),
                                                            HttpUtility.HtmlEncode(txtEmail.Text),
                                                            HttpUtility.HtmlEncode(txtPhone.Text),
                                                            Resource.Client_Feedback_Header,
                                                            HttpUtility.HtmlEncode(txtMessage.Text));

                mailTemplate.BuildMail();
                SendMail.SendMailNow(SettingsMail.EmailForFeedback, mailTemplate.Subject, mailTemplate.Body, true);

                MultiView1.SetActiveView(ViewEmailSend);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(new MetaInfo(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Client_Feedback_Header)),
                    null);
            liCaptcha.Visible = SettingsMain.EnableCaptcha;

            if (!Page.IsPostBack)
            {
                if (curentCustomer.RegistredUser)
                {
                    txtSenderName.Text = curentCustomer.LastName + ' ' + curentCustomer.FirstName;
                    txtEmail.Text = curentCustomer.EMail;
                    txtPhone.Text = curentCustomer.Phone;
                }
            }
        }
    }
}