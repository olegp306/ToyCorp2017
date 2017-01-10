using System;
using System.Web;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using Newtonsoft.Json;
using Resources;

namespace UserControls.OrderConfirmation
{
    public partial class StepShipping : System.Web.UI.UserControl
    {
        #region Fields

        public OrderConfirmationData PageData { get; set; }

        protected string DisplayBlock = "";
        protected string BlockCustomField = "";

        protected bool DisplayDelivery;
        protected string DeliveryJson = "";

        #endregion

        #region Public

        public void UpdatePageData(OrderConfirmationData orderConfirmationData)
        {
            if (orderConfirmationData.UserType != EnUserType.RegisteredUser)
            {
                orderConfirmationData.ShippingContact.Address = SettingsOrderConfirmation.IsShowAddress
                    ? HttpUtility.HtmlEncode(txtAddress.Text)
                    : string.Empty;

                orderConfirmationData.ShippingContact.Zip = SettingsOrderConfirmation.IsShowZip
                    ? HttpUtility.HtmlEncode(txtZip.Text)
                    : string.Empty;

                orderConfirmationData.ShippingContact.CustomField1 = SettingsOrderConfirmation.IsShowCustomShippingField1
                    ? HttpUtility.HtmlEncode(txtShippingField1.Text)
                    : string.Empty;

                orderConfirmationData.ShippingContact.CustomField2 = SettingsOrderConfirmation.IsShowCustomShippingField2
                    ? HttpUtility.HtmlEncode(txtShippingField2.Text)
                    : string.Empty;

                orderConfirmationData.ShippingContact.CustomField3 = SettingsOrderConfirmation.IsShowCustomShippingField3
                    ? HttpUtility.HtmlEncode(txtShippingField3.Text)
                    : string.Empty;
            }
        }

        public bool IsValidData(OrderConfirmationData orderConfirmationData)
        {
            var shippingItem = orderConfirmationData.SelectedShippingItem;

            if (shippingItem.Ext != null &&
               shippingItem.Ext.Type == ExtendedType.Pickpoint &&
                (shippingItem.Ext.PickpointAddress.IsNullOrEmpty() && shippingItem.Type != ShippingType.Cdek))
            {
                ((AdvantShopClientPage)Page).ShowMessage(Notify.NotifyType.Error, Resource.ShippingRates_ChoosePickPoint);
                return false;
            }

            if ((shippingItem.Type == ShippingType.ShippingByOrderPrice || shippingItem.Type == ShippingType.FixedRate)
                &&
                (String.IsNullOrEmpty(orderConfirmationData.ShippingContact.Address) || String.IsNullOrEmpty(orderConfirmationData.ShippingContact.Zip) || orderConfirmationData.ShippingContact.Zip.Length != 6))
            {
                ((AdvantShopClientPage)Page).ShowMessage(Notify.NotifyType.Error, Resource.ShippingRates_NeedAddress);
                return false;
            }

            return true;
        }

        #endregion

        #region Protected

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (PageData == null || PageData.ShippingContact == null)
                return;

            LoadShipping();
            SetRequiredFields();


            if (!SettingsOrderConfirmation.IsShowCustomShippingField1 &&
                !SettingsOrderConfirmation.IsShowCustomShippingField2 &&
                !SettingsOrderConfirmation.IsShowZip && !SettingsOrderConfirmation.IsShowAddress &&
                !SettingsOrderConfirmation.IsShowCustomShippingField3)
            {
                divDisplayAddress.Attributes.Add("style","display:none;");
            }

        }

        #endregion

        #region Private

        private void LoadShipping()
        {
            ShippingRates.CountryId = PageData.ShippingContact.CountryId;
            ShippingRates.Zip = PageData.ShippingContact.Zip;
            ShippingRates.City = PageData.ShippingContact.City;
            ShippingRates.Region = PageData.ShippingContact.RegionName;
            ShippingRates.Distance = PageData.Distance;
            ShippingRates.PickpointId = PageData.SelectedShippingItem.Ext != null ? PageData.SelectedShippingItem.Ext.PickpointId.TryParseInt() : 0;

            ShippingRates.SelectShippingOptionEx = PageData.SelectedShippingItem.Ext;
            ShippingRates.ShoppingCart = ShoppingCartService.CurrentShoppingCart;

            ShippingRates.LoadMethods(PageData.SelectedShippingItem.Id);

            if (ShippingRates.SelectedItem != null)
            {
                PageData.SelectedShippingItem = ShippingRates.SelectedItem;
                PageData.Distance = ShippingRates.Distance;
            }
            else
            {
                PageData.SelectedShippingItem = new ShippingItem();
                PageData.Distance = 0;
            }

            DisplayBlock = ShippingMethodService.ShowAddressField(PageData.UserType, ShippingRates.SelectedItem)
                ? "block"
                : "none";
            BlockCustomField = ShippingMethodService.ShowCustomField(PageData.UserType, ShippingRates.SelectedItem)
                ? "block"
                : "none";

            liDelivery.Text = string.Format("{0}, {1}{2}",
                PageData.ShippingContact.Country,
                (PageData.ShippingContact.RegionName != "" && PageData.ShippingContact.RegionName != PageData.ShippingContact.City
                    ? PageData.ShippingContact.RegionName + ", "
                    : ""),
                PageData.ShippingContact.City);

            DeliveryJson = JsonConvert.SerializeObject(new
            {
                countryId = PageData.ShippingContact.CountryId,
                country = PageData.ShippingContact.Country,
                region = PageData.ShippingContact.RegionName,
                city = PageData.ShippingContact.City
            });

            DisplayDelivery = PageData.UserType != EnUserType.RegisteredUser;
        }

        private void SetRequiredFields()
        {
            txtZip.ValidationType = SettingsOrderConfirmation.IsRequiredZip
                ? EValidationType.Required
                : EValidationType.None;

            txtAddress.ValidationType = SettingsOrderConfirmation.IsRequiredAddress
                ? EValidationType.Required
                : EValidationType.None;

            txtShippingField1.ValidationType = SettingsOrderConfirmation.IsReqCustomShippingField1
                ? EValidationType.Required
                : EValidationType.None;

            txtShippingField2.ValidationType = SettingsOrderConfirmation.IsReqCustomShippingField2
                ? EValidationType.Required
                : EValidationType.None;

            txtShippingField3.ValidationType = SettingsOrderConfirmation.IsReqCustomShippingField3
                ? EValidationType.Required
                : EValidationType.None;
        }

        #endregion
    }
}