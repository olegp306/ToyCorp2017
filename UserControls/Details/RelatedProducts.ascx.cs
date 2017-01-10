//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using AdvantShop.BonusSystem;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using Resources;
using AdvantShop;
using AdvantShop.Configuration;

namespace UserControls.Details
{
    public partial class RelatedProducts : System.Web.UI.UserControl
    {
        public List<int> ProductIds { get; set; }
        public bool HasProducts { get; private set; }
        public RelatedType RelatedType { get; set; }
        protected CustomerGroup customerGroup = CustomerContext.CurrentCustomer.CustomerGroup;
        protected int ImageMaxWidth = SettingsPictureSize.SmallProductImageWidth;
        protected int ImageMaxHeight = SettingsPictureSize.SmallProductImageHeight;


        protected bool DisplayBuyButton = SettingsCatalog.DisplayBuyButton;
        protected bool DisplayMoreButton = SettingsCatalog.DisplayMoreButton;
        protected bool DisplayPreOrderButton = SettingsCatalog.DisplayPreOrderButton;

        protected string BuyButtonText = SettingsCatalog.BuyButtonText;
        protected string MoreButtonText = SettingsCatalog.MoreButtonText;
        protected string PreOrderButtonText = SettingsCatalog.PreOrderButtonText;

        protected bool IsAdmin = CustomerContext.CurrentCustomer.IsAdmin;

        private float _discountByTime = DiscountByTimeService.GetDiscountByTime();
        private List<ProductDiscount> _productDiscountModels = null;
        private List<ProductLabel> _customLabels = null;

        protected string enablePhotoPreviews = SettingsCatalog.EnablePhotoPreviews.ToString().ToLower();
        protected int ColorImageHeight = SettingsPictureSize.ColorIconHeightCatalog;
        protected int ColorImageWidth = SettingsPictureSize.ColorIconWidthCatalog;
        protected bool EnableRating = SettingsCatalog.EnableProductRating;

        private BonusCard _bonusCard = null;

        public RelatedProducts()
        {
            ProductIds = new List<int>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ProductIds.Count == 0)
            {
                return;
            }

            LoadModules();

            var finalList = new List<Product>();

            foreach (var id in ProductIds)
            {
                finalList.AddRange(ProductService.GetRelatedProducts(id, RelatedType).Where(product => !ProductIds.Contains(product.ProductId) && finalList.All(item => product.ID != item.ID)));
            }

            lvRelatedProducts.DataSource = finalList.Distinct().OrderByDescending(p => p.MainPrice > 0).ThenByDescending(p => p.TotalAmount > 0 || p.AllowPreOrder);
            lvRelatedProducts.DataBind();
            HasProducts = lvRelatedProducts.Items.Any();



        }

        protected string RenderPictureTag(string urlPhoto, string productName, string photoDesc)
        {
            if (string.IsNullOrEmpty(urlPhoto))
                return "<img src=\"images/nophoto_small.jpg\" alt=\"\" />";
            return string.Format("<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" />",
                                 FoldersHelper.GetImageProductPath(ProductImageType.Small, urlPhoto, false),
                                 HttpUtility.HtmlEncode(photoDesc));
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
                productPrice = CatalogService.RenderPrice(price, totalDiscount, false, customerGroup);
            }
            else
            {
                productPrice = Resource.Client_Catalog_From + " " +
                               CatalogService.RenderPrice(price, totalDiscount, false, customerGroup, isWrap: true);
            }

            var bonusesPrice = showBonuses ? GetBonusPrice(price, totalDiscount) : string.Empty;

            return productPrice + bonusesPrice;
        }


        //protected string RenderPrice(float productPrice, float discount)
        //{
        //    if (productPrice == 0)
        //    {
        //        return string.Format("<div class=\'price\'>{0}</div>", Resource.Client_Catalog_ContactWithUs);
        //    }

        //    string res;

        //    float price = CatalogService.CalculateProductPrice(productPrice, 0, customerGroup, null);
        //    float priceWithDiscount = CatalogService.CalculateProductPrice(productPrice, discount, customerGroup, null);

        //    if (price.Equals(priceWithDiscount))
        //    {
        //        res = string.Format("<div class=\'price\'>{0}</div>", CatalogService.GetStringPrice(price));
        //    }
        //    else
        //    {
        //        res = string.Format("<div class=\"price-old\">{0}</div><div class=\"price\">{1}</div><div class=\"price-benefit\">{2} {3} {4} {5}% </div>",
        //                            CatalogService.GetStringPrice(productPrice),
        //                            CatalogService.GetStringPrice(priceWithDiscount),
        //                            Resource.Client_Catalog_Discount_Benefit,
        //                            CatalogService.GetStringPrice(price - priceWithDiscount),
        //                            Resource.Client_Catalog_Discount_Or,
        //                            CatalogService.FormatPriceInvariant(discount));
        //    }

        //    return res;
        //}

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

        protected string RenderLabels(int productId, bool recomend, bool sales, bool best, bool news, float discount,
          int labelCount = 5)
        {
            var labels = _customLabels.Where(l => l.ProductIds.Contains(productId)).Select(l => l.LabelCode).ToList();

            return CatalogService.RenderLabels(recomend, sales, best, news, discount, labelCount, labels);
        }

        private string GetBonusPrice(float price, float totalDiscount)
        {
            if (_bonusCard == null || price < 0)
                return string.Empty;

            return CatalogService.RenderBonusPrice(_bonusCard.BonusPercent, price, totalDiscount, customerGroup);
        }

        protected string RenderPictureTag(int productId, string strPhoto, string urlpath, string photoDesc, string productName, int photoId)
        {
            string alt = photoDesc.IsNotEmpty() ? photoDesc : productName + " - " + Resource.ClientPage_AltText + " " + photoId;
            return
                string.Format(
                    "<a href=\"{0}\" class=\"mp-pv-lnk\"><img src=\"{1}\" alt=\"{2}\" class=\"pv-photo p-photo scp-img {4}\" {3}></a>",
                    UrlService.GetLink(ParamType.Product, urlpath, productId), strPhoto.IsNotEmpty()
                                                                                   ? FoldersHelper.GetImageProductPath(ProductImageType.Small, strPhoto, false)
                                                                                   : "images/nophoto_small.jpg",
                    HttpUtility.HtmlEncode(alt),
                    InplaceEditor.Image.AttributesProduct(photoId == 0 ? productId : photoId, productId, ProductImageType.Small, true, !strPhoto.IsNullOrEmpty(), !strPhoto.IsNullOrEmpty()),
                    InplaceEditor.CanUseInplace(RoleActionKey.DisplayCatalog) ? "js-inplace-image-visible-permanent" : "");
        }


    }
}
