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


            mvDefaultPage.SetActiveView(defaultView);
            news.Visible = SettingsDesign.NewsVisibility;
            mainPageProduct.Visible = SettingsDesign.MainPageProductsVisibility;
            voting.Visible = SettingsDesign.VotingVisibility;
            checkOrder.Visible = SettingsDesign.CheckOrderVisibility;
            giftCertificate.Visible = SettingsDesign.GiftSertificateVisibility &&
                                      SettingsOrderConfirmation.EnableGiftCertificateService;
            carousel.Visible = SettingsDesign.CarouselVisibility;

            if (MainPageProductsAfterCarousel.IsNotEmpty())
                carousel.CssSlider += " flexslider-inline";

            SetMeta(null, string.Empty);

            if (GoogleTagManager.Enabled)
            {
                var tagManager = ((AdvantShopMasterPage)Master).TagManager;
                tagManager.PageType = GoogleTagManager.ePageType.home;
            }

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