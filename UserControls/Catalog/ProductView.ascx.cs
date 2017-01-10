//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using AdvantShop;
using AdvantShop.BonusSystem;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.CMS;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using Resources;

namespace UserControls.Catalog
{
    public partial class ProductView : UserControl
    {
        #region Fields

        public object DataSource { set; get; }
        public SettingsCatalog.ProductViewMode ViewMode { set; get; }
        public bool HasProducts { private set; get; }

        protected bool EnableRating = SettingsCatalog.EnableProductRating;
        protected bool EnableCompare = SettingsCatalog.EnableCompareProducts;
        protected CustomerGroup customerGroup = CustomerContext.CurrentCustomer.CustomerGroup;
        protected int ImageWidth = SettingsPictureSize.SmallProductImageWidth;
        protected int ImageHeightSmall = SettingsPictureSize.SmallProductImageHeight;
        protected int ImageHeightXSmall = SettingsPictureSize.XSmallProductImageHeight;

        protected bool DisplayBuyButton = SettingsCatalog.DisplayBuyButton;
        protected bool DisplayMoreButton = SettingsCatalog.DisplayMoreButton;
        protected bool DisplayPreOrderButton = SettingsCatalog.DisplayPreOrderButton;

        protected string BuyButtonText = SettingsCatalog.BuyButtonText;
        protected string MoreButtonText = SettingsCatalog.MoreButtonText;
        protected string PreOrderButtonText = SettingsCatalog.PreOrderButtonText;

        protected int ColorImageHeight = SettingsPictureSize.ColorIconHeightCatalog;
        protected int ColorImageWidth = SettingsPictureSize.ColorIconWidthCatalog;


        protected bool IsAdmin = CustomerContext.CurrentCustomer.IsAdmin;

        private float _discountByTime = DiscountByTimeService.GetDiscountByTime();
        private List<ProductDiscount> _productDiscountModels = null;
        private List<ProductLabel> _customLabels = null;

        private BonusCard _bonusCard = null;

        protected string enablePhotoPreviews = SettingsCatalog.EnablePhotoPreviews.ToString().ToLower();

        #endregion

        protected string RenderPictureTag(string urlPhoto, string additionalUrlPhoto, string urlPath, string photoDesc, string productName, int photoId, int productId)
        {
            string strFormat = string.Empty;

            string alt = photoDesc.IsNotEmpty() ? photoDesc : productName + " - " + Resource.ClientPage_AltText + " " + photoId;

            switch (ViewMode)
            {
                case SettingsCatalog.ProductViewMode.Tiles:
                    strFormat = string.Format("<a class=\"pv-lnk-photo\" href=\"{0}\"><img src=\"{1}\" alt=\"{2}\" class=\"scp-img p-photo {4}\" {3} /></a>", urlPath,
                        FoldersHelper.GetImageProductPath(ProductImageType.Small, additionalUrlPhoto.IsNotEmpty() ? additionalUrlPhoto : urlPhoto, false), 
                        HttpUtility.HtmlEncode(alt), 
                        InplaceEditor.Image.AttributesProduct(photoId == 0 ? productId : photoId, productId, ProductImageType.Small, true, !urlPhoto.IsNullOrEmpty(), !urlPhoto.IsNullOrEmpty()),
                        InplaceEditor.CanUseInplace(RoleActionKey.DisplayCatalog) ? "js-inplace-image-visible-permanent" : "");
                    break;
                case SettingsCatalog.ProductViewMode.List:
                    strFormat = string.Format("<a class=\"pv-lnk-photo\" href=\"{0}\"><img src=\"{1}\" alt=\"{2}\" class=\"scp-img p-photo {4}\" {3}/></a>", urlPath,
                        FoldersHelper.GetImageProductPath(ProductImageType.Small, additionalUrlPhoto.IsNotEmpty() ? additionalUrlPhoto : urlPhoto, false), 
                        HttpUtility.HtmlEncode(alt), 
                        InplaceEditor.Image.AttributesProduct(photoId == 0 ? productId : photoId, productId, ProductImageType.Small, true, !urlPhoto.IsNullOrEmpty(), !urlPhoto.IsNullOrEmpty()),
                        InplaceEditor.CanUseInplace(RoleActionKey.DisplayCatalog) ? "js-inplace-image-visible-permanent" : "");
                    break;
                case SettingsCatalog.ProductViewMode.Table:
                    if (urlPhoto.IsNotEmpty())
                    {
                        strFormat = string.Format("abbr=\"{0}\"",
                                                  FoldersHelper.GetImageProductPath(ProductImageType.Small, additionalUrlPhoto.IsNotEmpty() ? additionalUrlPhoto : urlPhoto, false));
                    }
                    break;
            }
            return strFormat;
        }

        protected string RenderPriceTag(int productId, float price, float discount, float multiPrice, bool showBonuses = false)
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

            string productPrice;

            if (multiPrice == 0)
            {
                productPrice = CatalogService.RenderPrice(price, totalDiscount, true, customerGroup);
            }
            else
            {
                productPrice = Resource.Client_Catalog_From + " " +
                               CatalogService.RenderPrice(price, totalDiscount, true, customerGroup, isWrap: true);
            }

            var bonusesPrice = showBonuses ? GetBonusPrice(price, totalDiscount) : string.Empty;

            return productPrice + bonusesPrice;
        }

        protected string RenderLabels(int productId, bool recomend, bool sales, bool best, bool news, float discount,
            int labelCount = 5)
        {
            var labels = _customLabels.Where(l => l.ProductIds.Contains(productId)).Select(l => l.LabelCode).ToList();

            return CatalogService.RenderLabels(recomend, sales, best, news, discount, labelCount, labels);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadModules();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            switch (ViewMode)
            {
                case SettingsCatalog.ProductViewMode.Tiles:
                    mvProducts.SetActiveView(viewTile);
                    lvTile.DataSource = DataSource;
                    lvTile.DataBind();
                    HasProducts = lvTile.Items.Any();
                    break;
                case SettingsCatalog.ProductViewMode.List:
                    mvProducts.SetActiveView(viewList);
                    lvList.DataSource = DataSource;
                    lvList.DataBind();
                    HasProducts = lvList.Items.Any();
                    break;

                case SettingsCatalog.ProductViewMode.Table:
                    mvProducts.SetActiveView(viewTable);
                    lvTable.DataSource = DataSource;
                    lvTable.DataBind();
                    HasProducts = lvTable.Items.Any();
                    break;
            }
        }

        private void LoadModules()
        {
            var discountModule = AttachedModules.GetModules<IDiscount>().FirstOrDefault();
            if (discountModule != null)
            {
                var classInstance = (IDiscount)Activator.CreateInstance(discountModule);
                _productDiscountModels = classInstance.GetProductDiscountsList();
            }

            _customLabels = new List<ProductLabel>();
            foreach (var labelModule in AttachedModules.GetModules<ILabel>())
            {
                var classInstance = (ILabel)Activator.CreateInstance(labelModule);
                var labelCode = classInstance.GetLabel();
                if (labelCode != null)
                    _customLabels.Add(labelCode);
            }

            var bonusSystem = AttachedModules.GetModules<IBonusSystem>().FirstOrDefault();
            if (bonusSystem != null)
            {
                if (CustomerContext.CurrentCustomer.BonusCardNumber != null)
                {
                    _bonusCard = BonusSystemService.GetCard(CustomerContext.CurrentCustomer.BonusCardNumber);
                }
                else if (BonusSystem.BonusFirstPercent != 0)
                {
                    _bonusCard = new BonusCard() { BonusPercent = BonusSystem.BonusFirstPercent };
                }
            }
        }

        private string GetBonusPrice(float price, float totalDiscount)
        {
            if (_bonusCard == null || price < 0)
                return string.Empty;

            return CatalogService.RenderBonusPrice(_bonusCard.BonusPercent, price, totalDiscount, customerGroup);
        }
    }
}