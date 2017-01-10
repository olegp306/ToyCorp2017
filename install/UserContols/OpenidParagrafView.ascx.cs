using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Helpers;

namespace ClientPages
{
    public partial class install_UserContols_OpenidParagrafView : AdvantShop.Controls.InstallerStep
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var providers = AdvantShop.Core.AdvantshopConfigService.GetActivityAuthProviders();
            fieldsetFacebook.Visible = !providers.ContainsKey("facebook") || providers["facebook"];
            fieldsetVk.Visible = !providers.ContainsKey("vkontakte") || providers["vkontakte"];
            fieldsetMailru.Visible = !providers.ContainsKey("mail.ru") || providers["mail.ru"];
            fieldsetGoogle.Visible = !providers.ContainsKey("google") || providers["google"];
            fieldsetYandex.Visible = !providers.ContainsKey("yandex") || providers["yandex"];
        }

        public new void LoadData()
        {
            String pass = Session["adminPass"] != null ? Session["adminPass"].ToString() : "";

            if (!string.IsNullOrEmpty(pass))
            {
                txtPass.Attributes.Add("value", pass);
                txtPassAgain.Attributes.Add("value", pass);
            }
            var cust = CustomerService.GetCustomersbyRole(Role.Administrator).FirstOrDefault();
            if (cust != null)
            {
                txtLogin.Text = cust.EMail;
            }
            chbGoogle.Checked = fieldsetGoogle.Visible && SettingsOAuth.GoogleActive;
            chbMailru.Checked = fieldsetMailru.Visible && SettingsOAuth.MailActive;
            chbYandex.Checked = fieldsetYandex.Visible && SettingsOAuth.YandexActive;
            chbVk.Checked = fieldsetVk.Visible && SettingsOAuth.VkontakteActive;
            chbFacebook.Checked = fieldsetFacebook.Visible && SettingsOAuth.FacebookActive;

            lblGoogleRedirectUrl.Text = SettingsMain.SiteUrl.TrimEnd('/') + "/Login.aspx?auth=google";
            if (fieldsetGoogle.Visible)
            {
                txtGoogleClientID.Text = SettingsOAuth.GoogleClientId;
                txtGoogleClientSecret.Text = SettingsOAuth.GoogleClientSecret;
            }

            if (fieldsetVk.Visible)
            {
                txtVKAppId.Text = SettingsOAuth.VkontakeClientId;
                txtVKSecret.Text = SettingsOAuth.VkontakeSecret;
            }

            if (fieldsetFacebook.Visible)
            {
                txtFacebookClientId.Text = SettingsOAuth.FacebookClientId;
                txtFacebookApplicationSecret.Text = SettingsOAuth.FacebookApplicationSecret;
            }

         
        }

        public new bool Validate()
        {
            if (string.IsNullOrEmpty(txtLogin.Text))
            {
                lblError.Text = Resources.Resource.Install_UserContols_OpenidParagrafView_NeedLogin;
                return false;
            }

            if (string.IsNullOrEmpty(txtPass.Text) && string.IsNullOrEmpty(txtPassAgain.Text))
            {
                lblError.Text = Resources.Resource.Install_UserContols_OpenidParagrafView_NeedPass;
                return false;
            }
            if (txtPass.Text != txtPassAgain.Text)
            {
                lblError.Text = Resources.Resource.Install_UserContols_OpenidParagrafView_WrongPass;
                return false;
            }

            var validList = new List<ValidElement>();
            if (chbFacebook.Checked)
            {
                validList.Add(new ValidElement
                {
                    Control = txtFacebookApplicationSecret,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = "Поле \"Application Secret\" обязательно для заполнения"
                });
                validList.Add(new ValidElement
                {
                    Control = txtFacebookClientId,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = "Поле \"Client Id\" обязательно для заполнения"
                });
            }
            if (chbVk.Checked)
            {
                validList.Add(new ValidElement
                {
                    Control = txtVKAppId,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = "Поле \"App Id\" обязательно для заполнения"
                });
                validList.Add(new ValidElement
                {
                    Control = txtVKSecret,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = "Поле \"Secret\" обязательно для заполнения"
                });
            }
         
            return ValidationHelper.Validate(validList);
        }

        public new void SaveData()
        {
            Session["adminPass"] = txtPass.Text.Trim();
            SettingsOAuth.GoogleActive = chbGoogle.Checked;
            SettingsOAuth.MailActive = chbMailru.Checked;
            SettingsOAuth.YandexActive = chbYandex.Checked;
            SettingsOAuth.VkontakteActive = chbVk.Checked;
            SettingsOAuth.FacebookActive = chbFacebook.Checked;

            SettingsOAuth.VkontakeClientId = txtVKAppId.Text;
            SettingsOAuth.VkontakeSecret = txtVKSecret.Text;

            SettingsOAuth.FacebookClientId = txtFacebookClientId.Text;
            SettingsOAuth.FacebookApplicationSecret = txtFacebookApplicationSecret.Text;


            if (string.IsNullOrEmpty(txtPass.Text) || string.IsNullOrEmpty(txtPassAgain.Text)) return;
            if (txtPass.Text != txtPassAgain.Text) return;


            var customer = CustomerService.GetCustomerByEmail("admin") ?? CustomerService.GetCustomerByEmail(txtLogin.Text);
            if (customer == null)
            {
                CustomerService.InsertNewCustomer(new Customer
                {
                    Password = txtPass.Text,
                    FirstName = "admin",
                    LastName = "admin",
                    Phone = string.Empty,
                    SubscribedForNews = false,
                    EMail = txtLogin.Text,
                    CustomerRole = Role.Administrator,
                    CustomerGroupId = 1,
                });
            }
            else
            {
                customer.EMail = txtLogin.Text;
                CustomerService.UpdateCustomer(customer);
                CustomerService.ChangePassword(customer.Id, txtPass.Text, false);
            }
        }
    }
}