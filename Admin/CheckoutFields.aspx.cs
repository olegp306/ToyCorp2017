//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using Resources;

namespace Admin
{
    public partial class CheckoutFields : AdvantShopAdminPage
    {
        protected void Page_PreRender(object sender, EventArgs e)
        {
            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_ContactFields_Header));

            // customer
            txtFirstName.Text = SettingsOrderConfirmation.CustomerFirstNameField;

            chkIsShowLastName.Checked = SettingsOrderConfirmation.IsShowLastName;
            chkIsReqLastName.Checked = SettingsOrderConfirmation.IsRequiredLastName;

            chkIsShowPatronymic.Checked = SettingsOrderConfirmation.IsShowPatronymic;
            chkIsReqPatronymic.Checked = SettingsOrderConfirmation.IsRequiredPatronymic;

            txtPhone.Text = SettingsOrderConfirmation.CustomerPhoneField;
            chkIsShowPhone.Checked = SettingsOrderConfirmation.IsShowPhone;
            chkIsReqPhone.Checked = SettingsOrderConfirmation.IsRequiredPhone;

            // checkout
            chkIsShowCountry.Checked = SettingsOrderConfirmation.IsShowCountry;
            chkIsReqCountry.Checked = SettingsOrderConfirmation.IsRequiredCountry;            

            chkIsShowState.Checked = SettingsOrderConfirmation.IsShowState;
            chkIsReqState.Checked = SettingsOrderConfirmation.IsRequiredState;

            chkIsShowCity.Checked = SettingsOrderConfirmation.IsShowCity;
            chkIsReqCity.Checked = SettingsOrderConfirmation.IsRequiredCity;

            chkIsShowZip.Checked = SettingsOrderConfirmation.IsShowZip;
            chkIsReqZip.Checked = SettingsOrderConfirmation.IsRequiredZip;

            chkIsShowAddress.Checked = SettingsOrderConfirmation.IsShowAddress;
            chkIsReqAddress.Checked = SettingsOrderConfirmation.IsRequiredAddress;

            chkIsShowUserAgreementText.Checked = SettingsOrderConfirmation.IsShowUserAgreementText;
            txtUserAgreementText.Text = SettingsOrderConfirmation.UserAgreementText;

            chkIsShowUserComment.Checked = SettingsOrderConfirmation.IsShowUserComment;

            txtCustomShippingField1.Text = SettingsOrderConfirmation.CustomShippingField1;
            chkIsShowCustomShippingField1.Checked = SettingsOrderConfirmation.IsShowCustomShippingField1;
            chkIsReqCustomShippingField1.Checked = SettingsOrderConfirmation.IsReqCustomShippingField1;

            txtCustomShippingField2.Text = SettingsOrderConfirmation.CustomShippingField2;
            chkIsShowCustomShippingField2.Checked = SettingsOrderConfirmation.IsShowCustomShippingField2;
            chkIsReqCustomShippingField2.Checked = SettingsOrderConfirmation.IsReqCustomShippingField2;

            txtCustomShippingField3.Text = SettingsOrderConfirmation.CustomShippingField3;
            chkIsShowCustomShippingField3.Checked = SettingsOrderConfirmation.IsShowCustomShippingField3;
            chkIsReqCustomShippingField3.Checked = SettingsOrderConfirmation.IsReqCustomShippingField3;

            // buy one click
            txtBuyInOneClickName.Text = SettingsOrderConfirmation.BuyInOneClickName;
            chkIsShowBuyInOneClickName.Checked = SettingsOrderConfirmation.IsShowBuyInOneClickName;
            chkIsRequiredBuyInOneClickName.Checked = SettingsOrderConfirmation.IsRequiredBuyInOneClickName;

            txtBuyInOneClickEmail.Text = SettingsOrderConfirmation.BuyInOneClickEmail;
            chkIsShowBuyInOneClickEmail.Checked = SettingsOrderConfirmation.IsShowBuyInOneClickEmail;
            chkIsRequiredBuyInOneClickEmail.Checked = SettingsOrderConfirmation.IsRequiredBuyInOneClickEmail;


            txtBuyInOneClickPhone.Text = SettingsOrderConfirmation.BuyInOneClickPhone;
            chkIsShowBuyInOneClickPhone.Checked = SettingsOrderConfirmation.IsShowBuyInOneClickPhone;
            chkIsRequiredBuyInOneClickPhone.Checked = SettingsOrderConfirmation.IsRequiredBuyInOneClickPhone;

            txtBuyInOneClickComment.Text = SettingsOrderConfirmation.BuyInOneClickComment;
            chkIsShowBuyInOneClickComment.Checked = SettingsOrderConfirmation.IsShowBuyInOneClickComment;
            chkIsRequiredBuyInOneClickComment.Checked = SettingsOrderConfirmation.IsRequiredBuyInOneClickComment;
        }
        
        protected void btnSave_OnClick(object sender, EventArgs e)
        {
            // customer
            SettingsOrderConfirmation.CustomerFirstNameField = txtFirstName.Text;

            SettingsOrderConfirmation.IsShowLastName = chkIsShowLastName.Checked;
            SettingsOrderConfirmation.IsRequiredLastName = chkIsReqLastName.Checked;

            SettingsOrderConfirmation.IsShowPatronymic = chkIsShowPatronymic.Checked;
            SettingsOrderConfirmation.IsRequiredPatronymic = chkIsReqPatronymic.Checked;

            SettingsOrderConfirmation.CustomerPhoneField = txtPhone.Text;
            SettingsOrderConfirmation.IsShowPhone = chkIsShowPhone.Checked;
            SettingsOrderConfirmation.IsRequiredPhone = chkIsReqPhone.Checked;

            // checkout
            SettingsOrderConfirmation.IsShowCountry = chkIsShowCountry.Checked;
            SettingsOrderConfirmation.IsRequiredCountry = chkIsReqCountry.Checked;

            SettingsOrderConfirmation.IsShowState = chkIsShowState.Checked;
            SettingsOrderConfirmation.IsRequiredState = chkIsReqState.Checked;

            SettingsOrderConfirmation.IsShowCity = chkIsShowCity.Checked;
            SettingsOrderConfirmation.IsRequiredCity = chkIsReqCity.Checked;

            SettingsOrderConfirmation.IsShowZip = chkIsShowZip.Checked;
            SettingsOrderConfirmation.IsRequiredZip = chkIsReqZip.Checked;

            SettingsOrderConfirmation.IsShowAddress = chkIsShowAddress.Checked;
            SettingsOrderConfirmation.IsRequiredAddress = chkIsReqAddress.Checked;

            SettingsOrderConfirmation.IsShowUserAgreementText = chkIsShowUserAgreementText.Checked;
            SettingsOrderConfirmation.UserAgreementText = txtUserAgreementText.Text;

            SettingsOrderConfirmation.IsShowUserComment = chkIsShowUserComment.Checked;

            SettingsOrderConfirmation.CustomShippingField1 = txtCustomShippingField1.Text;
            SettingsOrderConfirmation.IsShowCustomShippingField1 = chkIsShowCustomShippingField1.Checked;
            SettingsOrderConfirmation.IsReqCustomShippingField1 = chkIsReqCustomShippingField1.Checked;

            SettingsOrderConfirmation.CustomShippingField2 = txtCustomShippingField2.Text;
            SettingsOrderConfirmation.IsShowCustomShippingField2 = chkIsShowCustomShippingField2.Checked;
            SettingsOrderConfirmation.IsReqCustomShippingField2 = chkIsReqCustomShippingField2.Checked;

            SettingsOrderConfirmation.CustomShippingField3 = txtCustomShippingField3.Text;
            SettingsOrderConfirmation.IsShowCustomShippingField3 = chkIsShowCustomShippingField3.Checked;
            SettingsOrderConfirmation.IsReqCustomShippingField3 = chkIsReqCustomShippingField3.Checked;

            // buy one click
            SettingsOrderConfirmation.BuyInOneClickName = txtBuyInOneClickName.Text;
            SettingsOrderConfirmation.IsShowBuyInOneClickName = chkIsShowBuyInOneClickName.Checked;
            SettingsOrderConfirmation.IsRequiredBuyInOneClickName = chkIsRequiredBuyInOneClickName.Checked;

            SettingsOrderConfirmation.BuyInOneClickEmail = txtBuyInOneClickEmail.Text;
            SettingsOrderConfirmation.IsShowBuyInOneClickEmail = chkIsShowBuyInOneClickEmail.Checked;
            SettingsOrderConfirmation.IsRequiredBuyInOneClickEmail = chkIsRequiredBuyInOneClickEmail.Checked;


            SettingsOrderConfirmation.BuyInOneClickPhone = txtBuyInOneClickPhone.Text;
            SettingsOrderConfirmation.IsShowBuyInOneClickPhone = chkIsShowBuyInOneClickPhone.Checked;
            SettingsOrderConfirmation.IsRequiredBuyInOneClickPhone = chkIsRequiredBuyInOneClickPhone.Checked;

            SettingsOrderConfirmation.BuyInOneClickComment = txtBuyInOneClickComment.Text;
            SettingsOrderConfirmation.IsShowBuyInOneClickComment = chkIsShowBuyInOneClickComment.Checked;
            SettingsOrderConfirmation.IsRequiredBuyInOneClickComment = chkIsRequiredBuyInOneClickComment.Checked;
        }
    }
}