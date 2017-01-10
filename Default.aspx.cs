//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Helpers;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.SEO;

namespace ClientPages
{
    public partial class Default : AdvantShopClientPage
    {
        protected string MainPageProducts;
        protected string MainPageProductsAfterCarousel;

        protected void Page_Load(object sender, EventArgs e)
        {
            SettingsDesign.eMainPageMode currentMode = !Demo.IsDemoEnabled ||
                                                       !CommonHelper.GetCookieString("structure").IsNotEmpty()
                                                           ? SettingsDesign.MainPageMode
                                                           : (SettingsDesign.eMainPageMode)Enum.Parse(typeof(SettingsDesign.eMainPageMode), CommonHelper.GetCookieString("structure"));
            LoadModules();

            switch (currentMode)
            {
                case SettingsDesign.eMainPageMode.Default:
                    mvDefaultPage.SetActiveView(defaultView);
                    news.Visible = SettingsDesign.NewsVisibility;
                    mainPageProduct.Visible = SettingsDesign.MainPageProductsVisibility;
                    carousel.Visible = SettingsDesign.CarouselVisibility;

                    if (MainPageProductsAfterCarousel.IsNotEmpty())
                        carousel.CssSlider += " flexslider-inline";
                    break;
                case SettingsDesign.eMainPageMode.TwoColumns:
                    mvDefaultPage.SetActiveView(twoColumnsView);
                    newsTwoColumns.Visible = SettingsDesign.NewsVisibility;
                    votingTwoColumns.Visible = SettingsDesign.VotingVisibility;
                    checkOrderTwoColumns.Visible = SettingsDesign.CheckOrderVisibility;
                    giftCertificateTwoColumns.Visible = SettingsDesign.GiftSertificateVisibility &&
                                                        SettingsOrderConfirmation.EnableGiftCertificateService;
                    carouselTwoColumns.Visible = SettingsDesign.CarouselVisibility;
                    mainPageProductTwoColumns.Visible = SettingsDesign.MainPageProductsVisibility;

                    if (MainPageProductsAfterCarousel.IsNotEmpty())
                        carouselTwoColumns.CssSlider += " flexslider-inline-b";
                    break;
                case SettingsDesign.eMainPageMode.ThreeColumns:
                    mvDefaultPage.SetActiveView(threeColumnsView);
                    newsThreeColumns.Visible = SettingsDesign.NewsVisibility;
                    votingThreeColumns.Visible = SettingsDesign.VotingVisibility;
                    CheckOrderThreeColumns.Visible = SettingsDesign.CheckOrderVisibility;
                    giftCertificateThreeColumns.Visible = SettingsDesign.GiftSertificateVisibility &&
                                                          SettingsOrderConfirmation.EnableGiftCertificateService;
                    carouselThreeColumns.Visible = SettingsDesign.CarouselVisibility;
                    mainPageProductThreeColumn.Visible = SettingsDesign.MainPageProductsVisibility;
                    break;
            }

            SetMeta(null, string.Empty);

            if (GoogleTagManager.Enabled)
            {
                var tagManager = ((AdvantShopMasterPage)Master).TagManager;
                tagManager.PageType = GoogleTagManager.ePageType.home;
            }

            //filterPrice.CategoryId = _categoryId;
            filterPrice.CategoryId = AdvantShop.Catalog.CategoryService.GetCategoryIdByName("Каталог");
            // filterPrice.InDepth = Indepth;
            filterPrice.InDepth = true;
            // filterPrice.Visible = SettingsCatalog.ShowPriceFilter;
        }

        private void LoadModules()
        {
            foreach (var module in AttachedModules.GetModules<IRenderIntoMainPage>())
            {
                var classInstance = (IRenderIntoMainPage)Activator.CreateInstance(module, null);
                MainPageProducts += classInstance.RenderMainPageProducts();
                MainPageProductsAfterCarousel += classInstance.RenderMainPageAfterCarousel();
            }
        }
    }
}