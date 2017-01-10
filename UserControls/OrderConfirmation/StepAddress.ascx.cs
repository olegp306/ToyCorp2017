using System;
using System.Linq;
using System.Web;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Security;
using Resources;

namespace UserControls.OrderConfirmation
{
    public partial class StepAddress : System.Web.UI.UserControl
    {
        #region Fields

        public OrderConfirmationData PageData { get; set; }

        #endregion

        #region Public methods

        public bool IsValidData(OrderConfirmationData orderConfirmationData)
        {
            if (orderConfirmationData == null)
                return false;

            if (orderConfirmationData.UserType == EnUserType.RegisteredUser)
            {
                return orderConfirmationData.ShippingContact != null;
            }

            if (orderConfirmationData.UserType == EnUserType.RegisteredUserWithoutAddress)
            {
                return ValidateRegUserData();
            }

            var isValid = ValidateLogin(orderConfirmationData) && ValidateUserData() && ValidateShipping();
            if (!isValid)
            {
                ((AdvantShopClientPage)Page).ShowMessage(Notify.NotifyType.Error,
                    Resource.Client_OrderConfirmation_EnterEmptyField);
            }

            return isValid;
        }

        public void UpdatePageData(OrderConfirmationData orderConfirmationData)
        {
            if (orderConfirmationData.UserType == EnUserType.RegisteredUserWithoutAddress)
            {
                orderConfirmationData.Customer.EMail = CustomerContext.CurrentCustomer.EMail.Contains("@temp")
                    ? HttpUtility.HtmlEncode(txtRegEmail.Text)
                    : CustomerContext.CurrentCustomer.EMail;
                orderConfirmationData.Customer.FirstName = HttpUtility.HtmlEncode(txtRegFirstName.Text);
                orderConfirmationData.Customer.LastName = HttpUtility.HtmlEncode(txtRegLastName.Text);
                orderConfirmationData.Customer.Patronymic = HttpUtility.HtmlEncode(txtRegPatronymic.Text);
                orderConfirmationData.Customer.Phone = HttpUtility.HtmlEncode(txtRegPhone.Text);
            }

            if (orderConfirmationData.UserType == EnUserType.NoUser ||
                orderConfirmationData.UserType == EnUserType.NewUserWithOutRegistration ||
                orderConfirmationData.UserType == EnUserType.JustRegistredUser)
            {
                orderConfirmationData.Customer.EMail = HttpUtility.HtmlEncode(txtEmail.Text);
                orderConfirmationData.Customer.FirstName = HttpUtility.HtmlEncode(txtFirstName.Text);
                orderConfirmationData.Customer.LastName = HttpUtility.HtmlEncode(txtLastName.Text);
                orderConfirmationData.Customer.Patronymic = HttpUtility.HtmlEncode(txtPatronymic.Text);
                orderConfirmationData.Customer.Phone = HttpUtility.HtmlEncode(txtPhone.Text);

                orderConfirmationData.BillingContact.Name = orderConfirmationData.Customer.FirstName +
                                                            (orderConfirmationData.Customer.Patronymic.IsNotEmpty() ? " " + orderConfirmationData.Customer.Patronymic : "") +
                                                            (orderConfirmationData.Customer.LastName.IsNotEmpty() ? " " + orderConfirmationData.Customer.LastName : "");

                if (chknewcustomer.Checked)
                {
                    orderConfirmationData.Customer.Password = txtNewPassword.Text.Trim();
                    orderConfirmationData.UserType = EnUserType.JustRegistredUser;
                }
            }
        }

        #endregion

        #region Protected Methods

        protected void Page_PreRender(object sender, EventArgs e) // Page_Load
        {
            if (PageData == null)
                return;

            if (SettingsMain.EnablePhoneMask)
            {
                txtPhone.CssClass = "mask-phone mask-inp";
            }

            if (PageData.UserType == EnUserType.RegisteredUser || PageData.UserType == EnUserType.RegisteredUserWithoutAddress)
            {
                DivNoReg.Visible = false;
                LoadDataForRegistredUser();
            }
            else if (PageData.UserType != EnUserType.RegisteredUser)
            {
                DivReg.Visible = false;
                DivRegWithoutAddress.Visible = false;
                tblLoginTable.Visible = PageData.UserType == EnUserType.JustRegistredUser;

                LoadDataForUnRegisteredUser();

                signInSocial.Visible = LoginOpenID.Visible;
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            var email = txtLoginEmail.Text.Trim();
            var password = txtLoginPassword.Text.Trim();

            if (!(email.IsNullOrEmpty() || password.IsNullOrEmpty()))
            {
                if (!AuthorizeService.SignIn(email, password, false, true))
                {
                    ((AdvantShopClientPage) Page).ShowMessage(Notify.NotifyType.Error, Resource.Client_MasterPage_WrongPassword);
                    txtLoginPassword.Text = "";
                    rbbOldCustomer.Checked = true;
                    rbNewCustomer.Checked = false;
                }
                else
                    Response.Redirect("~/orderconfirmation.aspx");
            }
        }

        #endregion

        #region Private Methods

        private void SetRequiredFields()
        {
            txtCountry.ValidationType = SettingsOrderConfirmation.IsRequiredCountry
                ? EValidationType.Required
                : EValidationType.None;

            txtRegion.ValidationType = SettingsOrderConfirmation.IsRequiredState
                ? EValidationType.Required
                : EValidationType.None;

            txtCity.ValidationType = SettingsOrderConfirmation.IsRequiredCity
                ? EValidationType.Required
                : EValidationType.None; 


            txtLastName.ValidationType = SettingsOrderConfirmation.IsRequiredLastName
                ? EValidationType.Required
                : EValidationType.None;

            txtPatronymic.ValidationType = SettingsOrderConfirmation.IsRequiredPatronymic
                ? EValidationType.Required
                : EValidationType.None;

            txtPhone.ValidationType = SettingsOrderConfirmation.IsRequiredPhone
                ? EValidationType.Required
                : EValidationType.None;
        }

        private void SetRegistredRequiredFields()
        {
            txtRegEmail.ValidationType = liRegEmail.Text.Contains("@temp")
                ? EValidationType.NewEmail
                : EValidationType.Email;

            txtRegLastName.ValidationType = SettingsOrderConfirmation.IsRequiredLastName
                ? EValidationType.Required
                : EValidationType.None;

            txtRegPatronymic.ValidationType = SettingsOrderConfirmation.IsRequiredPatronymic
                ? EValidationType.Required
                : EValidationType.None;

            txtRegPhone.ValidationType = SettingsOrderConfirmation.IsRequiredPhone
                ? EValidationType.Required
                : EValidationType.None;
        }

        private void SetRegistredModalRequiredFields()
        {
            txtContactZoneOc.ValidationType = SettingsOrderConfirmation.IsRequiredState
                ? EValidationType.Required
                : EValidationType.None;

            txtContactCityOc.ValidationType = SettingsOrderConfirmation.IsRequiredCity
                ? EValidationType.Required
                : EValidationType.None;

            txtContactAddressOc.ValidationType = SettingsOrderConfirmation.IsRequiredAddress
                ? EValidationType.Required
                : EValidationType.None;

            txtContactZipOc.ValidationType = SettingsOrderConfirmation.IsRequiredZip
                ? EValidationType.Required
                : EValidationType.None;
        }

        private void LoadDataForUnRegisteredUser()
        {
            SetRequiredFields();

            if (PageData.ShippingContact == null || PageData.BillingContact == null || PageData.Customer == null)
            {
                PageData.BillingIsShipping = true;
                PageData.BillingContact = PageData.ShippingContact = new CustomerContact
                {
                    CustomerGuid = Guid.Empty,
                    Name = string.Empty,
                    Country = SettingsDesign.DisplayCityInTopPanel ? IpZoneContext.CurrentZone.CountryName : string.Empty,
                    CountryId = SettingsDesign.DisplayCityInTopPanel ? IpZoneContext.CurrentZone.CountryId : SettingsMain.SellerCountryId,
                    City = SettingsDesign.DisplayCityInTopPanel ? IpZoneContext.CurrentZone.City : txtCity.Text,
                    RegionName = SettingsDesign.DisplayCityInTopPanel ? IpZoneContext.CurrentZone.Region : string.Empty,
                };

                PageData.Customer = new Customer() {Id = CustomerContext.CurrentCustomer.Id};
            }
            else
            {
                if (PageData.Customer != null)
                {
                    txtEmail.Text = PageData.Customer.EMail;
                    txtFirstName.Text = PageData.Customer.FirstName;
                    txtLastName.Text = PageData.Customer.LastName;
                    txtPatronymic.Text = PageData.Customer.Patronymic;
                    txtPhone.Text = PageData.Customer.Phone;
                }
            }
        }

        private void LoadDataForRegistredUser()
        {
            dvLoginPanel.Visible = false;

            var customer = CustomerContext.CurrentCustomer;

            if (customer.Contacts.Count != 0)
            {
                DivRegWithoutAddress.Visible = false;

                cboCountryOc.DataSource = CountryService.GetAllCountries();
                cboCountryOc.DataBind();

                if (cboCountryOc.Items.FindByValue(IpZoneContext.CurrentZone.CountryId.ToString()) != null)
                    cboCountryOc.SelectedValue = IpZoneContext.CurrentZone.CountryId.ToString();

                hfOcContactShippingId.Value = customer.Contacts[0].CustomerContactID.ToString();
            }
            else
            {
                DivReg.Visible = false;
            }

            var shippingContact =
                customer.Contacts.FirstOrDefault(
                    contact => contact.CustomerContactID.ToString() == hfOcContactShippingId.Value);

            if (shippingContact == null)
            {
                shippingContact = new CustomerContact()
                {
                    Name = CustomerContext.CurrentCustomer.FirstName + " " + CustomerContext.CurrentCustomer.LastName,
                    Country = SettingsDesign.DisplayCityInTopPanel ? IpZoneContext.CurrentZone.CountryName : string.Empty ,
                    CountryId = SettingsDesign.DisplayCityInTopPanel ? IpZoneContext.CurrentZone.CountryId : SettingsMain.SellerCountryId,
                    RegionName = SettingsDesign.DisplayCityInTopPanel ? IpZoneContext.CurrentZone.Region : string.Empty,
                    City = SettingsDesign.DisplayCityInTopPanel ? IpZoneContext.CurrentZone.City : SettingsMain.City
                };

                liRegEmail.Text = customer.EMail;
                txtRegFirstName.Text = customer.FirstName;
                txtRegLastName.Text = customer.LastName;
                txtRegPatronymic.Text = customer.Patronymic;
                txtRegPhone.Text = customer.Phone;

                SetRegistredRequiredFields();

                PageData.UserType = EnUserType.RegisteredUserWithoutAddress;

                if (customer.EMail.Contains("@temp"))
                {
                    liRegEmail.Visible = false;
                    txtRegEmail.Visible = true;
                }
                else
                {
                    liRegEmail.Visible = true;
                    txtRegEmail.Visible = false;
                }
            }
            else
            {
                SetRegistredModalRequiredFields();
                PageData.UserType = EnUserType.RegisteredUser;
            }

            PageData.BillingIsShipping = true;
            PageData.ShippingContact = PageData.BillingContact = shippingContact;
            PageData.Customer = customer;

        }

        #region Validation

        private bool ValidateLogin(OrderConfirmationData orderConfirmationData)
        {
            var boolIsValidPast = true;
            if (orderConfirmationData.UserType != EnUserType.RegisteredUser)
            {
                txtEmail.Text = txtEmail.Text.Trim();
                if (txtEmail.Text.IsNullOrEmpty() || !ValidationHelper.IsValidEmail(txtEmail.Text))
                {
                    boolIsValidPast = false;
                    ((AdvantShopClientPage)Page).ShowMessage(Notify.NotifyType.Error, Resource.Client_OrderConfirmation_EnterValidEmail);
                }
            }

            if (orderConfirmationData.UserType == EnUserType.JustRegistredUser)
            {
                txtNewPassword.Text = txtNewPassword.Text.Trim();
                if (txtNewPassword.Text.IsNullOrEmpty() || txtNewPassword.Text.Length < 6)
                {
                    ((AdvantShopClientPage)Page).ShowMessage(Notify.NotifyType.Error, Resource.Client_Registration_PasswordLenght);
                    boolIsValidPast = false;
                }
            }
            return boolIsValidPast;
        }

        private bool ValidateUserData()
        {
            return !(txtFirstName.Text.IsNullOrEmpty() ||
                   (SettingsOrderConfirmation.IsShowLastName && SettingsOrderConfirmation.IsRequiredLastName && txtLastName.Text.IsNullOrEmpty()) ||
                   (SettingsOrderConfirmation.IsShowPatronymic && SettingsOrderConfirmation.IsRequiredPatronymic && txtPatronymic.Text.IsNullOrEmpty()));
        }

        private bool ValidateShipping()
        {
            return
                !((SettingsOrderConfirmation.IsShowPhone && SettingsOrderConfirmation.IsRequiredPhone && txtPhone.Text.IsNullOrEmpty()));
        }

        private bool ValidateRegUserData()
        {
            if (CustomerContext.CurrentCustomer.EMail.Contains("@temp") &&
                (txtRegEmail.Text.Trim().IsNullOrEmpty() || !ValidationHelper.IsValidEmail(txtRegEmail.Text)))
            {
                ((AdvantShopClientPage)Page).ShowMessage(Notify.NotifyType.Error,
                    Resource.Client_OrderConfirmation_EnterValidEmail);
                return false;
            }

            return !(txtRegFirstName.Text.IsNullOrEmpty() ||
                     (SettingsOrderConfirmation.IsShowLastName && SettingsOrderConfirmation.IsRequiredLastName && txtRegLastName.Text.IsNullOrEmpty()) ||
                     (SettingsOrderConfirmation.IsShowPatronymic && SettingsOrderConfirmation.IsRequiredPatronymic && txtRegPatronymic.Text.IsNullOrEmpty()));
        }

        #endregion

        #endregion
    }
}