//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Customers;
using System.Collections.Generic;
using AdvantShop.Orders;

namespace UserControls
{
    public partial class BuyInOneClick : System.Web.UI.UserControl
    {
        public int ProductId;
        public int OfferID;
        public List<CustomOption> CustomOptions;
        public List<OptionItem> SelectedOptions;

        protected BuyInOneclickPage PageEnum;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SettingsMain.EnablePhoneMask)
            {
                txtBuyOneClickPhone.CssClass = "mask-phone mask-inp";
            }

            if (CustomerContext.CurrentCustomer.RegistredUser)
            {
                hfProductId.Value = ProductId.ToString();
                txtBuyOneClickPhone.Text = CustomerContext.CurrentCustomer.Phone;
                txtBuyOneClickName.Text = CustomerContext.CurrentCustomer.FirstName + " " + CustomerContext.CurrentCustomer.LastName;
            }

            if (Request.Url.AbsolutePath.ToLower().Contains("details"))
            {
                PageEnum = BuyInOneclickPage.details;
            }
            else if (Request.Url.AbsolutePath.ToLower().Contains("shoppingcart"))
            {
                PageEnum = BuyInOneclickPage.shoppingcart;
            }
            else
            {
                PageEnum = BuyInOneclickPage.orderconfirmation;
            }

            SetRequiredFields();
        }

        private void SetRequiredFields()
        {
            txtBuyOneClickName.ValidationType = SettingsOrderConfirmation.IsRequiredBuyInOneClickName
                ? EValidationType.Required
                : EValidationType.None;

            txtBuyOneClickEmail.ValidationType = SettingsOrderConfirmation.IsRequiredBuyInOneClickEmail
                ? EValidationType.Email
                : EValidationType.None;

            txtBuyOneClickPhone.ValidationType = SettingsOrderConfirmation.IsRequiredBuyInOneClickPhone
                ? EValidationType.Required
                : EValidationType.None;

            txtBuyOneClickComment.ValidationType = SettingsOrderConfirmation.IsRequiredBuyInOneClickComment
                ? EValidationType.Required
                : EValidationType.None;
        }
    }
}