//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.FilePath;



namespace Social
{
    public partial class MasterPage : AdvantShopMasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            form.Action = Request.RawUrl;
           
            vk.Visible = !SettingsGeneral.AbsoluteUrl.Contains("localhost");

            Logo.ImgSource = FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.LogoImageName, false);

            if (SettingsDesign.MainPageMode == SettingsDesign.eMainPageMode.Default)
            {
                menuTop.Visible = true;
                searchBig.Visible = false;
                menuCatalog.Visible = true;
                menuTopMainPage.Visible = false;
            }
            else
            {
                menuTop.Visible = false;
                searchBig.Visible = (SettingsDesign.SearchBlockLocation == SettingsDesign.eSearchBlockLocation.TopMenu);
                menuCatalog.Visible = false;
                menuTopMainPage.Visible = true;

                liViewCss.Text = "<link rel=\"stylesheet\" href=\"social/css/views/twocolumns.css\" >";
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            ClearCssJsFiles();
            TemplateName = "social";

            AddCssFiles(new[]
                {
                    "~/css/normalize.css",
                    "~/css/advcss/modal.css",
                    "~/css/advcss/notify.css",
                    
                    "~/css/jq/jquery.cloud-zoom.css",
                    "~/css/jq/jquery-ui-1.8.17.custom.css",
                    "~/css/jq/jquery.autocomplete.css",
                    "~/css/jq/jquery.fancybox-1.3.4.css",
                    "~/css/theme.css",
                    "~/css/constructor.css",
                    "~/css/carousel.css",
                    "~/css/forms.css",
                    "~/css/styles.css?p=20151228",
                    "~/css/styles-extra.css",
                    "~/css/validator.css",
                    "~/js/plugins/jpicker/css/jpicker.css",
                    "~/js/plugins/upper/css/upper.css",
                    "~/js/plugins/expander/css/expander.css",
                    "~/js/plugins/vote/css/vote.css",
                    "~/js/plugins/progress/css/progress.css",
                    "~/js/plugins/compare/css/compare.css",
                    "~/js/plugins/spinbox/css/spinbox.css",
                    "~/js/plugins/cart/css/cart.css",
                    "~/js/plugins/scrollbar/css/scrollbar.css",
                    "~/js/plugins/tabs/css/tabs.css",
                    "~/js/plugins/flexslider/css/flexslider.css",
                    "~/js/plugins/sizeColorPickerDetails/css/sizeColorPickerDetails.css",
                    "~/js/plugins/sizeColorPickerCatalog/css/sizeColorPickerCatalog.css",

                    "~/social/css/vk.css"
                });

            AddJsFilesTop(new[]
                {
                    "~/js/jq/jquery-1.7.1.min.js",
                    "~/js/modernizr.custom.js",
                    "~/js/ejs_fulljslint.js",
                    "~/js/ejs.js"
                });

            AddJsFilesBottom(new[]
                {
                    "~/js/localization/" + SettingsMain.Language + "/lang.js",
                    "~/js/string.format-1.0.js",

                    "~/js/advantshop.js",
                    "~/js/services/Utilities.js",
                    "~/js/services/scriptsManager.js",
                    "~/js/services/jsuri-1.1.1.js",
                    "~/js/services/offers.js",

                    "~/js/jq/jquery-ui-1.8.17.custom.min.js",
                    "~/js/jq/jquery.cloud-zoom.1.0.2.js",
                    "~/js/jq/jquery.cookie.js",
                    "~/js/jq/jquery.metadata.js",
                    "~/js/jq/jquery.fancybox-1.3.4.js",
                    "~/js/jq/jquery.jcarousel.min.js",
                    "~/js/jq/jquery.placeholder.js",
                    "~/js/jq/jquery.validate.js",
                    "~/js/jq/jquery.autocomplete.js",
                    "~/js/jq/jquery.raty.js",
                    "~/js/jq/jquery.mousewheel.js",
                    "~/js/advjs/advDetectTouch.js",
                    "~/js/advjs/advFeedback.js",
                    "~/js/advjs/advNotify.js",
                    "~/js/advjs/advModal.js",
                    "~/js/advjs/advMoveCaret.js",
                    "~/js/advjs/advGiftCertificate.js",
                    "~/js/advjs/advMyAccount.js",
                    "~/js/advjs/advOrderConfirmation.js",
                    "~/js/advjs/advUtils.js",


                    "~/js/jq/jquery.inputmask.js",
                    "~/js/jq/jquery.inputmask.date.extensions.js",
                    "~/js/plugins/cart/cart.js",
                    "~/js/plugins/compare/compare.js",
                    "~/js/plugins/expander/expander.js",
                    "~/js/plugins/jpicker/jpicker.js",
                    "~/js/plugins/progress/progress.js",
                    "~/js/plugins/reviews/reviews.js",
                    "~/js/plugins/scrollbar/scrollbar.js",
                    "~/js/plugins/spinbox/spinbox.js",
                    "~/js/plugins/tabs/tabs.js",
                    "~/js/plugins/upper/upper.js",
                    "~/js/plugins/vote/vote.js",
                    "~/js/plugins/videos/videos.js",
                    "~/js/plugins/flexslider/flexslider.js",
                    "~/js/plugins/sizeColorPickerDetails/sizeColorPickerDetails.js",
                    "~/js/plugins/sizeColorPickerCatalog/sizeColorPickerCatalog.js",
                    "~/js/plugins/imagePicker/imagePicker.js",

                    "~/js/jspage/details/details.js",

                    "~/js/common.js",
                    "~/js/constructor.js",
                    "~/js/dopostback.js",
                    "~/js/validateInit.js",

                    "~/social/js/social.js"
                });
        }
    }
}