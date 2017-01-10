//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web;
using AdvantShop;
using AdvantShop.BonusSystem;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Customers;
using AdvantShop.Mails;
using AdvantShop.SEO;
using AdvantShop.Security;
using Resources;

namespace ClientPages
{
    public partial class Registration : AdvantShopClientPage
    {
        protected BonusCard Card;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && CustomerContext.CurrentCustomer.RegistredUser)
            {
                Response.Redirect("default.aspx");
            }

            SetRequiredFields();            

            if (!Page.IsPostBack && Demo.IsDemoEnabled)
            {
                txtEmail.Text = Demo.GetRandomEmail();
                dvDemoDataUserNotification.Visible = true;
                txtFirstName.Text = Demo.GetRandomName();
                txtLastName.Text = Demo.GetRandomLastName();
                txtPhone.Text = Demo.GetRandomPhone();
            }

            if (SettingsMain.EnablePhoneMask)
            {
                txtPhone.CssClass = "mask-phone mask-inp";
            }

            if (BonusSystem.IsActive)
            {
                var bonusCard = Session["bonuscard"] != null ? Session["bonuscard"].ToString() : null;
                if (bonusCard.IsNotEmpty())
                {
                    Card = BonusSystemService.GetCard(bonusCard.TryParseLong(true));
                }

                if (BonusSystem.BonusesForNewCard != 0)
                {
                    liBonusesForNewCard.Text = string.Format(Resource.Client_StepBonus_NewCardBonuses,
                        CatalogService.GetStringPrice(BonusSystem.BonusesForNewCard));
                    liBonusesForNewCard.Visible = true;
                }
            }

            NewsSubscription.Visible = SettingsDesign.NewsSubscriptionVisibility;

            liCaptcha.Visible = SettingsMain.EnableCaptcha;

            SetMeta(
                new MetaInfo(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Client_Registration_Registration)),
                string.Empty);
        }        

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (!DataValidation()) return;

            long? bonusCardNumber = null;
            var bonusCard = Session["bonuscard"] != null ? Session["bonuscard"].ToString() : null;
            if (bonusCard.IsNotEmpty())
            {
                var card = BonusSystemService.GetCard(bonusCard.TryParseLong(true));
                if (card != null)
                {
                    bonusCardNumber = card.CardNumber;
                }
                Session["bonuscard"] = null;
            }

            lblMessage.Visible = false;

            CustomerService.InsertNewCustomer(new Customer
            {
                CustomerGroupId = CustomerGroupService.DefaultCustomerGroup,
                Password = HttpUtility.HtmlEncode(txtPassword.Text),
                FirstName = HttpUtility.HtmlEncode(txtFirstName.Text),
                LastName =
                    SettingsOrderConfirmation.IsShowLastName ? HttpUtility.HtmlEncode(txtLastName.Text) : string.Empty,
                Patronymic =
                    SettingsOrderConfirmation.IsShowPatronymic
                        ? HttpUtility.HtmlEncode(txtPatronymic.Text)
                        : string.Empty,
                Phone = SettingsOrderConfirmation.IsShowPhone ? HttpUtility.HtmlEncode(txtPhone.Text) : string.Empty,
                SubscribedForNews = chkSubscribed4News.Checked,
                EMail = HttpUtility.HtmlEncode(txtEmail.Text),
                CustomerRole = Role.User,
                BonusCardNumber = bonusCardNumber
            });

            AuthorizeService.SignIn(txtEmail.Text, txtPassword.Text, false, true);

            //------------------------------------------

            var regMailTemplate = new RegistrationMailTemplate(SettingsMain.SiteUrl,
                                                               HttpUtility.HtmlEncode(txtFirstName.Text),
                                                               HttpUtility.HtmlEncode(txtLastName.Text),
                                                               AdvantShop.Localization.Culture.ConvertDate(DateTime.Now),
                                                               HttpUtility.HtmlEncode(txtPassword.Text),
                                                               chkSubscribed4News.Checked
                                                                   ? Resource.Client_Registration_Yes
                                                                   : Resource.Client_Registration_No,
                                                               HttpUtility.HtmlEncode(txtEmail.Text));
            regMailTemplate.BuildMail();

            if (CustomerContext.CurrentCustomer.IsVirtual)
            {
                ShowMessage(Notify.NotifyType.Error,
                            Resource.Client_Registration_Whom + txtEmail.Text + '\r' + Resource.Client_Registration_Text +
                            regMailTemplate.Body);
            }
            else
            {
                SendMail.SendMailNow(txtEmail.Text, regMailTemplate.Subject, regMailTemplate.Body, true);
                SendMail.SendMailNow(SettingsMail.EmailForRegReport, regMailTemplate.Subject, regMailTemplate.Body, true);
            }

            Response.Redirect("myaccount.aspx");
        }

        private bool DataValidation()
        {
            var boolIsValidPast = true;

            boolIsValidPast &= txtPasswordConfirm.Text.Trim().IsNotEmpty() && txtPassword.Text.Trim().IsNotEmpty() &&
                               txtPassword.Text == txtPasswordConfirm.Text;

            boolIsValidPast &= txtPassword.Text.Length >= 6;

            if ((string.IsNullOrEmpty(txtPasswordConfirm.Text)) || (string.IsNullOrEmpty(txtPassword.Text)) ||
                (txtPassword.Text != txtPasswordConfirm.Text))
            {
                ShowMessage(Notify.NotifyType.Error, Resource.Client_Registration_PasswordNotMatch);
            }
            if (txtPassword.Text.Length < 6)
            {
                ShowMessage(Notify.NotifyType.Error, Resource.Client_Registration_PasswordLenght);
            }

            if (SettingsOrderConfirmation.IsShowPhone && SettingsOrderConfirmation.IsRequiredPhone &&
                txtPhone.Text.IsNullOrEmpty())
                boolIsValidPast = false;

            if (SettingsOrderConfirmation.IsShowLastName && SettingsOrderConfirmation.IsRequiredLastName &&
                txtLastName.Text.IsNullOrEmpty())
                boolIsValidPast = false;

            if (SettingsOrderConfirmation.IsShowPatronymic && SettingsOrderConfirmation.IsRequiredPatronymic &&
                txtPatronymic.Text.IsNullOrEmpty())
                boolIsValidPast = false;

            boolIsValidPast &= txtFirstName.Text.Trim().IsNotEmpty();            
            boolIsValidPast &= AdvantShop.Helpers.ValidationHelper.IsValidEmail(txtEmail.Text);            

            if (!dnfValid.IsValid())
            {
                boolIsValidPast = false;
                ShowMessage(Notify.NotifyType.Error, Resource.Client_Registration_CodeDiffrent);
            }

            if (CustomerService.CheckCustomerExist(txtEmail.Text))
            {
                boolIsValidPast = false;
                ShowMessage(Notify.NotifyType.Error, Resource.Client_Registration_CustomerExist);
            }

            if (SettingsOrderConfirmation.IsShowUserAgreementText && !chkAgree.Checked)
            {
                boolIsValidPast = false;
                ShowMessage(Notify.NotifyType.Error, Resource.Client_Registration_MustAgree);
            }

            if (!boolIsValidPast)
            {
                dnfValid.TryNew();
                return false;
            }
            return true;
        }

        private void SetRequiredFields()
        {
            txtLastName.ValidationType = SettingsOrderConfirmation.IsRequiredLastName
                ? EValidationType.Required
                : EValidationType.None;

            txtPatronymic.ValidationType = SettingsOrderConfirmation.IsRequiredPatronymic
                ? EValidationType.Required
                : EValidationType.None;

            txtPhone.ValidationType = BonusSystem.IsActive || SettingsOrderConfirmation.IsRequiredPhone
                ? EValidationType.Required
                : EValidationType.None;
        }
    }
}