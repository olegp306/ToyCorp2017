//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using AdvantShop;
using AdvantShop.CMS;
using AdvantShop.Controls;
using AdvantShop.Core.UrlRewriter;
using Resources;
using AdvantShop.SEO;


using AdvantShop.FilePath;
using System.Web;
using AdvantShop.Customers;
using AdvantShop.Catalog;
using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.Helpers;
using AdvantShop.Design;

namespace ClientPages
{
    public partial class StaticPageView : AdvantShopClientPage
    {
        #region Fields

        public SettingsDesign.eMainPageMode Mode { set; get; }

        protected bool EnableRating = SettingsCatalog.EnableProductRating;
        protected CustomerGroup customerGroup = CustomerContext.CurrentCustomer.CustomerGroup;

        protected int ImageMaxHeight = SettingsPictureSize.SmallProductImageHeight;
        protected int ImageMaxWidth = SettingsPictureSize.SmallProductImageWidth;

        protected bool DisplayBuyButton = SettingsCatalog.DisplayBuyButton;
        protected bool DisplayMoreButton = SettingsCatalog.DisplayMoreButton;
        protected bool DisplayPreOrderButton = SettingsCatalog.DisplayPreOrderButton;

        protected string BuyButtonText = SettingsCatalog.BuyButtonText;
        protected string MoreButtonText = SettingsCatalog.MoreButtonText;
        protected string PreOrderButtonText = SettingsCatalog.PreOrderButtonText;

        protected int ItemCountInRow = SettingsDesign.CountProductInLine;
        protected string enablePhotoPreviews = SettingsCatalog.EnablePhotoPreviews.ToString().ToLower();

        private float _discountByTime = DiscountByTimeService.GetDiscountByTime();
        private List<ProductDiscount> _productDiscountModels = null;

        protected int ColorImageHeight = SettingsPictureSize.ColorIconHeightCatalog;
        protected int ColorImageWidth = SettingsPictureSize.ColorIconWidthCatalog;


        #endregion


        protected StaticPage page;
        protected bool hasSubPages;
        protected MetaInfo metaInfo;
        protected void Page_Load(object sender, EventArgs e)
        {
            int pageId = Page.Request["staticpageid"].TryParseInt();
            page = StaticPageService.GetStaticPage(pageId);
            if (pageId == 0 || page == null || (page != null && !page.Enabled))
            {
                Error404();
                return;
            }

            metaInfo = SetMeta(page.Meta, page.PageName);

            ucBreadCrumbs.Items =
                StaticPageService.GetParentStaticPages(pageId)
                                 .Select(StaticPageService.GetStaticPage)
                                 .Select(stPage => new BreadCrumbs
                                     {
                                         Name = stPage.PageName,
                                         Url =
                                             UrlService.GetLink(ParamType.StaticPage, stPage.UrlPath,
                                                                stPage.StaticPageId)
                                     }).Reverse().ToList();

            ucBreadCrumbs.Items.Insert(0, new BreadCrumbs
                {
                    Name = Resource.Client_MasterPage_MainPage,
                    Url = UrlService.GetAbsoluteLink("/")
                });

            if (GoogleTagManager.Enabled)
            {
                var tagManager = ((AdvantShopMasterPage)Master).TagManager;
                tagManager.PageType = GoogleTagManager.ePageType.info;
            }

            //Добавляем логику Новинки поппулярные итд
            AddNewContentInFooter();
            
        }

        private void AddNewContentInFooter()
        {
            SettingsDesign.eMainPageMode currentMode = !Demo.IsDemoEnabled || !CommonHelper.GetCookieString("structure").IsNotEmpty()
                                                           ? SettingsDesign.MainPageMode
                                                           : (SettingsDesign.eMainPageMode)Enum.Parse(typeof(SettingsDesign.eMainPageMode), CommonHelper.GetCookieString("structure"));


            if (SettingsDesign.Template == TemplateService.DefaultTemplateId && Mode != currentMode)
            {
                this.Visible = false;
                return;
            }

            LoadModules();

            var countNew = ProductOnMain.GetProductCountByType(ProductOnMain.TypeFlag.New);
            var countDiscount = ProductOnMain.GetProductCountByType(ProductOnMain.TypeFlag.Discount);
            var countBestseller = ProductOnMain.GetProductCountByType(ProductOnMain.TypeFlag.Bestseller);
            var countRecomended = ProductOnMain.GetProductCountByType(ProductOnMain.TypeFlag.Recomended);

            switch (Mode)
            {
                case SettingsDesign.eMainPageMode.Default:

                    mvMainPageProduct.SetActiveView(viewDefault);

                    int ItemsCount = 3;

                    if (countBestseller == 0)
                    {
                        ItemsCount = 3;
                    }
                    if (countNew == 0)
                    {
                        ItemsCount = ItemsCount == 2 ? 3 : 6;
                    }
                    if (countDiscount == 0)
                    {
                        ItemsCount = ItemsCount == 2 ? 3 : 6;
                    }
                    if (countRecomended == 0)
                    {
                        ItemsCount = ItemsCount == 2 ? 3 : 6;
                    }

                    if (countBestseller > 0)
                    {
                        liBestsellers.Attributes.Add("class", "block width-for-" + ItemsCount); //SettingsDesign.CountLineOnMainPage);
                        //lvBestSellers.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.Bestseller, ItemCountInRow);
                        lvBestSellers.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.Bestseller, 12);
                        lvBestSellers.DataBind();
                    }
                    else
                    {
                        liBestsellers.Visible = false;
                    }
                    if (countNew > 0)
                    {
                        liNew.Attributes.Add("class", "block width-for-" + ItemsCount); //SettingsDesign.CountLineOnMainPage);
                        //lvNew.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.New, ItemCountInRow);
                        lvNew.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.New, 12);
                        lvNew.DataBind();
                    }
                    else
                    {
                        liNew.Visible = false;
                    }

                    if (countDiscount > 0)
                    {
                        liDiscount.Attributes.Add("class", "block block-last width-for-" + ItemsCount); //SettingsDesign.CountLineOnMainPage);
                        //lvDiscount.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.OnSale, ItemCountInRow);
                        lvDiscount.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.OnSale, 12);
                        lvDiscount.DataBind();
                    }
                    else
                    {
                        liDiscount.Visible = false;
                    }

                    if (countRecomended > 0)
                    {
                        liRecomended.Attributes.Add("class", "block width-for-" + ItemsCount); //SettingsDesign.CountLineOnMainPage);
                        //lvRecomended.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.Recomended, ItemCountInRow);
                        lvRecomended.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.Recomended, 12);
                        lvRecomended.DataBind();
                    }
                    else
                    {
                        liRecomended.Visible = false;
                    }

                    break;
                case SettingsDesign.eMainPageMode.TwoColumns:
                    mvMainPageProduct.SetActiveView(viewDefault);

                    if (countBestseller > 0)
                    {
                        lvBestSellers.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.Bestseller, ItemCountInRow);
                        lvBestSellers.DataBind();
                    }
                    else
                    {
                        pnlBest.Visible = false;
                    }

                    if (countNew > 0)
                    {
                        lvNewAlternative.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.New, ItemCountInRow);
                        lvNewAlternative.DataBind();
                    }
                    else
                    {
                        pnlNew.Visible = false;
                    }

                    if (countDiscount > 0)
                    {
                        lvDiscountAlternative.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.Discount, ItemCountInRow);
                        lvDiscountAlternative.DataBind();
                    }
                    else
                    {
                        pnlDiscount.Visible = false;
                    }

                    if (countRecomended > 0)
                    {
                        lvRecomended.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.Recomended, ItemCountInRow);
                        lvRecomended.DataBind();
                    }
                    else
                    {
                        pnlRecomended.Visible = false;
                    }
                    break;
                case SettingsDesign.eMainPageMode.ThreeColumns:
                    mvMainPageProduct.SetActiveView(viewDefault);

                    if (countBestseller > 0)
                    {
                        lvBestSellers.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.Bestseller, ItemCountInRow);
                        lvBestSellers.DataBind();
                    }
                    else
                    {
                        pnlBest.Visible = false;
                    }

                    if (countNew > 0)
                    {
                        lvNewAlternative.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.New, ItemCountInRow);
                        lvNewAlternative.DataBind();
                    }
                    else
                    {
                        pnlNew.Visible = false;
                    }

                    if (countDiscount > 0)
                    {
                        lvDiscountAlternative.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.Discount, ItemCountInRow);
                        lvDiscountAlternative.DataBind();
                    }
                    else
                    {
                        pnlDiscount.Visible = false;
                    }

                    if (countRecomended > 0)
                    {
                        lvRecomended.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.Recomended, ItemCountInRow);
                        lvRecomended.DataBind();
                    }
                    else
                    {
                        pnlRecomended.Visible = false;
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var pages = StaticPageService.GetChildStaticPages(page.ID, true);

            if(!pages.Any())
                pages = StaticPageService.GetChildStaticPages(page.ParentId, true);

            if (pages.Any())
            {
                lvSubPages.DataSource = pages;
                lvSubPages.DataBind();
                hasSubPages = rightBlock.Visible = true;
            }
            else
            {
                hasSubPages = rightBlock.Visible = false;
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
        }

        protected string RenderPictureTag(int productId, string strPhoto, string urlpath, string photoDesc, string productName, int photoId)
        {
            string alt = photoDesc.IsNotEmpty() ? photoDesc : productName + " - " + Resource.ClientPage_AltText + " " + photoId;
            return
                string.Format(
                    "<a href=\"{0}\" class=\"mp-pv-lnk\"><img src=\"{1}\" alt=\"{2}\" class=\"pv-photo p-photo scp-img {5}\" {3} {4}></a>",
                    UrlService.GetLink(ParamType.Product, urlpath, productId), strPhoto.IsNotEmpty()
                                                                                   ? FoldersHelper.GetImageProductPath(ProductImageType.Small, strPhoto, false)
                                                                                   : "images/nophoto_small.jpg",
                    HttpUtility.HtmlEncode(alt), Mode == SettingsDesign.eMainPageMode.Default ? "style=\"max-width:100%;\"" : "",
                    InplaceEditor.Image.AttributesProduct(photoId == 0 ? productId : photoId, productId, ProductImageType.Small, true, !strPhoto.IsNullOrEmpty(), !strPhoto.IsNullOrEmpty()),
                    InplaceEditor.CanUseInplace(RoleActionKey.DisplayCatalog) ? "js-inplace-image-visible-permanent" : "");
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

            return CatalogService.RenderPrice(price, totalDiscount, true, customerGroup);
        }
    }
}