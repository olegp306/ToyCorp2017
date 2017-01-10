using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.Orders;

namespace ClientPages
{
    public partial class CompareProducts : AdvantShopClientPage
    {
        protected CustomerGroup customerGroup = CustomerContext.CurrentCustomer.CustomerGroup;
        protected float DiscountByTime = DiscountByTimeService.GetDiscountByTime();

        protected List<Property> CurrentProperties = PropertyService.GetPropertyNamesByCompareCart();
        protected List<Product> CurrentProducts;

        protected bool DisplayBuyButton = SettingsCatalog.DisplayBuyButton;
        protected bool DisplayPreOrderButton = SettingsCatalog.DisplayPreOrderButton;

        protected string BuyButtonText = SettingsCatalog.BuyButtonText;
        protected string PreOrderButtonText = SettingsCatalog.PreOrderButtonText;

        private float _discountByTime = DiscountByTimeService.GetDiscountByTime();
        private List<ProductDiscount> _productDiscountModels = null;

        protected int ImageSmallHeight = SettingsPictureSize.SmallProductImageHeight;
        protected int ImageSmallWidth = SettingsPictureSize.SmallProductImageWidth;

        protected void Page_Load(object sender, EventArgs e)
        {
            var compareProducts = ShoppingCartService.CurrentCompare;

            LoadModules();

            lvProperties.DataSource = CurrentProperties;
            lvProperties.DataBind();

            CurrentProducts = compareProducts.Select(item => item.Offer.Product).ToList();
            lvProducts.DataSource = compareProducts; // CurrentProducts;
            lvProducts.DataBind();
        }

        protected string RenderPictureTag(int productId, string strPhoto, string urlpath)
        {
            return
                string.Format(
                    "<a href=\"{0}\" class=\"compare-link-picture\"><img src=\"{1}\" class=\"compare-picture\"></a>",
                    UrlService.GetLink(ParamType.Product, urlpath, productId), strPhoto.IsNotEmpty()
                                                                                   ? FoldersHelper.GetImageProductPath(
                                                                                       ProductImageType.Small, strPhoto,
                                                                                       false)
                                                                                   : "images/nophoto_small.jpg");

        }

        private void LoadModules()
        {
            var discountModule = AttachedModules.GetModules<IDiscount>().FirstOrDefault();
            if (discountModule != null)
            {
                var classInstance = (IDiscount) Activator.CreateInstance(discountModule);
                _productDiscountModels = classInstance.GetProductDiscountsList();
            }
        }

        protected string RenderPriceTag(int productId, float price, float discount)
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

            return CatalogService.RenderPrice(price, totalDiscount, false, customerGroup);
        }
    }
}