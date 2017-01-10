using System;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Orders;
using Resources;

namespace UserControls.OrderConfirmation
{
    public partial class StepConfirm : System.Web.UI.UserControl
    {
        #region Fields

        public OrderConfirmationData PageData { get; set; }

        #endregion

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (PageData == null)
                return;

            if (IsPostBack && SettingsMain.EnableCaptcha)
            {
                dnfValid.TryNew();
            }
        }

        public bool IsValidData()
        {
            var boolValidationResult = true;

            if (SettingsMain.EnableCaptcha && !dnfValid.IsValid())
            {
                boolValidationResult = false;
                ((AdvantShopClientPage)this.Page).ShowMessage(Notify.NotifyType.Error, Resource.Client_Registration_CodeDiffrent);
            }

            var shpCrt = ShoppingCartService.CurrentShoppingCart;

            foreach (var item in shpCrt)
            {
                if (item.Amount > item.Offer.Amount && SettingsOrderConfirmation.AmountLimitation && !item.Offer.Product.AllowPreOrder)
                {
                    boolValidationResult = false;
                    ((AdvantShopClientPage)this.Page).ShowMessage(Notify.NotifyType.Error, Resource.Client_Items_Count_Error);
                }
            }

            return boolValidationResult;
        }

        public void UpdatePageData(OrderConfirmationData orderConfirmationData)
        {
            orderConfirmationData.CustomerComment = HttpUtility.HtmlEncode(txtComments.Text);
        }
    }
}