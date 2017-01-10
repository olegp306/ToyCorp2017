using AdvantShop.Catalog;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.Orders;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.FilePath;
using AdvantShop.Trial;

namespace UserControls.MasterPage
{
    public partial class ToolbarBottom : System.Web.UI.UserControl
    {
        protected int RecentlyCount = 0;
        protected int CompareCount = 0;
        protected int WishlistCount = 0;
        protected float CartCount = 0;
        protected bool EnableRating = SettingsCatalog.EnableProductRating;
        private readonly float _discountByTime = DiscountByTimeService.GetDiscountByTime();
        private List<ProductDiscount> _productDiscountModels = null;
        protected CustomerGroup CustomerGroup = CustomerContext.CurrentCustomer.CustomerGroup;

        protected bool ShowConfirmButton = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SettingsDesign.DisplayToolBarBottom)
            {
                this.Visible = false;
                return;
            }

            LoadModules();

            toolbarBottomWishlist.Visible = SettingsDesign.WishListVisibility;
            toolbarBottomInplace.Visible = CustomerContext.CurrentCustomer.IsAdmin || CustomerContext.CurrentCustomer.IsModerator || TrialService.IsTrialEnabled;
            toolbarBottomCompare.Visible = SettingsCatalog.EnableCompareProducts;
            toolbarBottomRecently.Visible = SettingsDesign.RecentlyViewVisibility;

            var recentlyItems = RecentlyViewService.LoadViewDataByCustomer(CustomerContext.CustomerId, 3);

            RecentlyCount = recentlyItems.Count;
            CompareCount = ShoppingCartService.CurrentCompare.Count;
            WishlistCount = ShoppingCartService.CurrentWishlist.Count;
            CartCount = ShoppingCartService.CurrentShoppingCart.TotalItems;

            if (RecentlyCount > 0)
            {
                lvToolbarBottomRecentlyView.DataSource = recentlyItems;
                lvToolbarBottomRecentlyView.DataBind();
            }

            foreach (var module in AttachedModules.GetModules<IRenderIntoShoppingCart>())
            {
                var moduleObject = (IRenderIntoShoppingCart) Activator.CreateInstance(module, null);
                ShowConfirmButton &= moduleObject.ShowConfirmButtons;
            }
        }

        protected string RenderPriceTag(int productId, float price, float discount, float multiPrice)
        {
            float totalDiscount = discount != 0 ? discount : _discountByTime;

            if (_productDiscountModels != null)
            {
                var prodDiscount = _productDiscountModels.Find(d => d.ProductId == productId);
                if (prodDiscount != null)
                {
                    totalDiscount = prodDiscount.Discount;
                }
            }

            if (multiPrice == 0)
                return CatalogService.RenderPrice(price, totalDiscount, false, CustomerGroup);

            return Resources.Resource.Client_Catalog_From + " " +
                   CatalogService.RenderPrice(price, totalDiscount, false, CustomerGroup, isWrap: true);
        }

        protected string RenderPictureTag(string urlPhoto, string urlPath, string photoDesc, string productName)
        {
            string strFormat = string.Empty;

            string alt = photoDesc.IsNotEmpty() ? photoDesc : productName;

            strFormat = string.Format("<a href=\"{0}\"><img src=\"{1}\" alt=\"{2}\" /></a>", urlPath,
                FoldersHelper.GetImageProductPath(ProductImageType.XSmall, urlPhoto, false), HttpUtility.HtmlEncode(alt));

            return strFormat;
        }

        private void LoadModules()
        {
            var discountModule = AttachedModules.GetModules<IDiscount>().FirstOrDefault();
            if (discountModule != null)
            {
                var classInstance = (IDiscount)Activator.CreateInstance(discountModule);
                _productDiscountModels = classInstance.GetProductDiscountsList();
            }
        }
    }
}
