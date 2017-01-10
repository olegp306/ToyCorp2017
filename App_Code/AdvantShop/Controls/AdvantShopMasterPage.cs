//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Customers;
using AdvantShop.Repository;
using AdvantShop.SEO;
using AdvantShop.Trial;

namespace AdvantShop.Controls
{
    public abstract class AdvantShopMasterPage : MasterPage
    {
        protected bool IsAdmin = CustomerContext.CurrentCustomer.IsAdmin
                                 || CustomerContext.CurrentCustomer.IsModerator
                                 || TrialService.IsTrialEnabled;

        #region Messages
        private StringBuilder NotifyMessage = new StringBuilder();

        public void ShowMessage(Notify.NotifyType notifyType, string message)
        {
            NotifyMessage.Append(Notify.FormatMessage(notifyType, message));
        }

        public string DisplayMessages()
        {
            return NotifyMessage.ToString();
        }

        #endregion

        #region Google Tag Manager

        public GoogleTagManager TagManager = new GoogleTagManager();

        #endregion

        #region js and css

        public string TemplateName { get; set; }

        private List<string> _JsTopList = new List<string>
            {
                "~/js/jq/jquery-1.7.1.min.js",
                "~/js/modernizr.custom.js",
                "~/js/ejs_fulljslint.js",
                "~/js/ejs.js",
                "~/js/plugins/bubble/bubble.js",
                "~/js/services/bubbleLocation.js"
            };

        private List<string> _JsBottomList = new List<string>
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
                "~/js/jq/jquery.inputmask.js",
                "~/js/jq/jquery.inputmask.date.extensions.js",
                "~/js/jq/jquery.maskedinput.min.js",
                


                "~/js/jq/jquery-file-upload/jquery.ui.widget.js",
                "~/js/jq/jquery-file-upload/jquery.iframe-transport.js",
                "~/js/jq/jquery-file-upload/jquery.fileupload.js",
                 
                "~/js/advjs/advBuyInOneClick.js",
                "~/js/advjs/advDetectTouch.js",
                "~/js/advjs/advFeedback.js",
                "~/js/advjs/advNotify.js",
                "~/js/advjs/advModal.js",
                "~/js/advjs/advMoveCaret.js",
                "~/js/advjs/advGiftCertificate.js",
                "~/js/advjs/advMyAccount.js",
                "~/js/advjs/advOrderConfirmation.js",
                "~/js/advjs/advUtils.js",
                 
                "~/js/plugins/cart/cart.js",
                "~/js/plugins/compare/compare.js",
                "~/js/plugins/expander/expander.js",
                "~/js/plugins/inplace/inplace.js",
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
                "~/js/plugins/cexpander/cexpander.js",
                "~/js/plugins/location/location.js",
                "~/js/plugins/vis/vis.js",
                "~/js/plugins/wishlist/wishlist.js",
                 
                "~/js/jspage/details/details.js",
                "~/js/jspage/compareproducts/compareproducts.js",
                "~/js/jspage/registration/registration.js",
                 
                "~/js/toolbarBottom.js",
                "~/js/tracking.js",
                "~/js/common.js",
                "~/js/constructor.js",
                "~/js/dopostback.js",
                "~/js/validateInit.js",
                "~/admin/js/masterpage/achievementsHelp.js"
            };

        private List<string> _CssTopList = new List<string>
            {
                "~/css/normalize.css",
                "~/css/advcss/modal.css",
                "~/css/advcss/notify.css",
                "~/css/staticblocks.css",
                 
                "~/css/jq/jquery.cloud-zoom.css",
                "~/css/jq/jquery-ui-1.8.17.custom.css",
                "~/css/jq/jquery.autocomplete.css",
                "~/css/jq/jquery.fancybox-1.3.4.css",
                 
                "~/css/theme.css",
                "~/css/constructor.css",
                "~/css/carousel.css",
                "~/css/forms.css",
                "~/css/styles.css?p=20151228",
                "~/css/styles-extra.css?p=20151223",
                "~/css/validator.css",
                "~/css/toolbar-bottom.css",
                "~/css/views/compareproducts.css",
                 
                "~/js/plugins/inplace/css/inplace.css",
                "~/js/plugins/location/css/location.css",
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
                "~/js/plugins/videos/css/videos.css",
                "~/js/plugins/flexslider/css/flexslider.css",
                "~/js/plugins/sizeColorPickerDetails/css/sizeColorPickerDetails.css",
                "~/js/plugins/sizeColorPickerCatalog/css/sizeColorPickerCatalog.css",
                "~/js/plugins/cexpander/css/cexpander.css",
                "~/js/plugins/bubble/css/bubble.css",
                "~/js/plugins/vis/css/vis.css",
                "~/js/plugins/wishlist/css/wishlist.css",
                 
                "~/js/jspage/details/css/details.css",
                 
                "~/admin/css/new_admin/achievementsHelp.css",

                "~/css/extra.css",
                
            };

        public string CssTop { get; set; }
        public string JsTop { get; set; }
        public string JsBottom { get; set; }

        public void ClearCssJsFiles()
        {
            _CssTopList.Clear();
            _JsTopList.Clear();
            _JsBottomList.Clear();
        }

        public void AddJsFilesTop(string[] fileNames)
        {
            _JsTopList.AddRange(fileNames);
        }

        public void AddJsFilesBottom(string[] fileNames)
        {
            _JsBottomList.AddRange(fileNames);
        }

        public void AddCssFiles(string[] fileNames)
        {
            _CssTopList.AddRange(fileNames);
        }

        public void RemoveFiles(string[] fileNames)
        {
            foreach (var name in fileNames)
            {
                _CssTopList.Remove(name);
                _JsTopList.Remove(name);
                _JsBottomList.Remove(name);
            }
        }

        #endregion

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            JsCssTool.ReCreateIfNotExist();
            CssTop = JsCssTool.MiniCss(_CssTopList, TemplateName.IsNullOrEmpty() ? "all.css" : TemplateName + "_all.css");
            JsTop = JsCssTool.MiniJs(_JsTopList, TemplateName.IsNullOrEmpty() ? "libs.js" : TemplateName + "_libs.js");
            JsBottom = JsCssTool.MiniJs(_JsBottomList, TemplateName.IsNullOrEmpty() ? "all.js" : TemplateName + "_all.js");
        }

        public string GetCssClassForDocument()
        {
            var classes = new List<string>();
            
            if (IsAdmin)
            {
                classes.Add(SettingsMain.EnableInplace ? "inplace-enabled" : "inplace-disabled");
            }

            if (SettingsDesign.DisplayToolBarBottom)
            {
                classes.Add("toolbar-enabled");
            }

            return classes.AggregateString(' ');
        }
    
      
    }
}
