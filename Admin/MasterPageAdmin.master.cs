//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Customers;
using AdvantShop.Notifications;
using AdvantShop.Security;
using AdvantShop.Statistic;

namespace Admin
{
    public partial class MasterPageAdmin : MasterPage
    {
        protected int newReviewsCount = 0;
        protected int newAdminMessage = 0;

        public bool AchievementsHelpVisible
        {
            get
            {
                return AchievementsHelp.Visible;
            }
            set
            {
                AchievementsHelp.isVisible = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lBase.Text = string.Format("<base href='{0}'/>",
                                       Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath +
                                       (!Request.ApplicationPath.EndsWith("/") ? "/" : string.Empty)
                                       + "admin/");


            MenuAdmin.CurrentCustomer = CustomerContext.CurrentCustomer;

            newAdminMessage = AdminMessagesService.GetNotViewedAdminMessagesCount();
            newReviewsCount = StatisticService.GetLastReviewsCount();

            adminMessages.CssClass = "top-part-right icon-mail " + (newAdminMessage > 0 ? "icon-selected" : "");
            adminMessages.Text = newAdminMessage > 0 ? newAdminMessage.ToString() : "";
            adminMessages.Visible = AdvantshopConfigService.GetLocalization() == "ru-RU";

            adminReviews.CssClass = "top-part-right icon-bubble " + (newReviewsCount > 0 ? "icon-selected" : "");
            adminReviews.Text = newReviewsCount > 0 ? newReviewsCount.ToString() : "";

            var _customer = CustomerContext.CurrentCustomer;
            if (_customer.CustomerRole == Role.Moderator)
            {
                var actions = RoleActionService.GetCustomerRoleActionsByCustomerId(_customer.Id);
                bool visible = actions.Any(a => a.Key == RoleActionKey.DisplayAdminMainPageStatistics && a.Enabled);

                StoreLanguage.Visible = visible;
                LastAdminMessages.Visible = visible;
                adminMessages.Visible = visible;

                adminReviews.Visible = actions.Any(a => a.Key == RoleActionKey.DisplayComments && a.Enabled);
            }
        }

        public void Page_PreRender(object sender, EventArgs e)
        {
            JsCssTool.ReCreateIfNotExist();

            headStyle.Text = JsCssTool.MiniCss(new List<string>{
                                    "~/css/validator.css"
                                   ,"~/css/normalize.css"
                                   ,"~/css/advcss/modal.css"
                                   ,"~/css/jq/jquery.autocomplete.css"
                                   ,"~/js/plugins/progress/css/progress.css"
                                   ,"~/js/plugins/jpicker/css/jpicker.css"
                                   ,"~/js/plugins/tabs/css/tabs.css"
                                   ,"~/js/plugins/bubble/css/bubble.css"
                                   ,"~/admin/css/jquery.tooltip.css"
                                   ,"~/admin/css/AdminStyle.css"
                                   ,"~/admin/css/advcss/notify.css"
                                   ,"~/admin/css/catalogDataTreeStyles.css"
                                   ,"~/admin/css/exportFeedStyles.css"
                                   ,"~/admin/js/plugins/tooltip/css/tooltip.css"
                                   ,"~/admin/js/plugins/placeholder/css/placeholder.css"
                                   ,"~/admin/js/plugins/radiolist/css/radiolist.css"
                                   ,"~/admin/js/plugins/chart/css/chart.css"
                                   ,"~/admin/js/plugins/noticeStatistic/css/noticeStatistic.css"
                                   ,"~/admin/js/plugins/help/css/help.css"
                                   ,"~/admin/js/plugins/datepicker/css/datepicker.css"
                                   ,"~/admin/js/plugins/transformer/css/transformer.css"

                                   ,"~/admin/js/jspage/adminmessages/css/styles.css?p=20151228"
                                   ,"~/admin/css/new_admin/buttons.css"
                                   ,"~/admin/css/new_admin/dropdown-menu.css"
                                   ,"~/admin/css/new_admin/icons.css"
                                   ,"~/admin/css/new_admin/admin.css"
                                   ,"~/admin/css/new_admin/pagenumber.css"
                                   ,"~/admin/css/new_admin/achievements.css"
                                   ,"~/admin/css/new_admin/achievementsHelp.css"
                                   ,"~/admin/css/new_admin/modules.css"},
                                   "admincss.css");

            headScript.Text = JsCssTool.MiniJs(new List<string>{
                                    "~/js/jq/jquery-1.7.1.min.js"
                                    ,"~/js/modernizr.custom.js"
                                    ,"~/js/localization/" + SettingsMain.Language + "/lang.js"
                                    ,"~/js/ejs_fulljslint.js"
                                    ,"~/js/ejs.js"},
                                    "adminlib.js");

            bottomScript.Text = JsCssTool.MiniJs(new List<string>{
                                      "~/js/jq/jquery.validate.js"
                                      ,"~/js/validateInit.js"
                                      ,"~/js/string.format-1.0.js"
                                      ,"~/js/jq/jquery.autocomplete.js"
                                      ,"~/js/jq/jquery.metadata.js"
                                      ,"~/js/advjs/advNotify.js"
                                      ,"~/js/advjs/advModal.js"
                                      ,"~/js/advjs/advTabs.js"
                                      ,"~/js/advjs/advUtils.js"
                                      ,"~/js/advantshop.js"
                                      ,"~/js/services/Utilities.js"
                                      ,"~/js/services/scriptsManager.js"
                                      ,"~/js/services/jsuri-1.1.1.js"
                                      ,"~/js/plugins/progress/progress.js"
                                      ,"~/js/plugins/jpicker/jpicker.js"
                                      ,"~/js/plugins/tabs/tabs.js"
                                      ,"~/js/plugins/bubble/bubble.js"
                                      ,"~/admin/js/customValidate.js"
                                      ,"~/admin/js/smallThings.js"
                                      ,"~/admin/js/jspage/adminmessages/adminmessages.js"
                                      ,"~/admin/js/jspage/achievements/achievements.js"
                                      ,"~/admin/js/jspage/vieworder.js"
                                      ,"~/admin/js/jspage/modulesmanager.js"
                                      ,"~/admin/js/jspage/default.js"
                                      ,"~/admin/js/jquery.cookie.min.js"
                                      ,"~/admin/js/jquery.qtip.min.js"
                                      ,"~/admin/js/jquery.tooltip.min.js"
                                      ,"~/admin/js/slimbox2.js"
                                      ,"~/admin/js/jquery.history.js"
                                      ,"~/admin/js/jquerytimer.js"
                                      ,"~/admin/js/admin.js"
                                      ,"~/admin/js/grid.js"
                                      ,"~/admin/js/plugins/tooltip/tooltip.js"
                                      ,"~/admin/js/plugins/placeholder/placeholder.js"
                                      ,"~/admin/js/plugins/chart/jquery.flot.js"
                                      ,"~/admin/js/plugins/chart/jquery.flot.pie.js"
                                      ,"~/admin/js/plugins/chart/jquery.flot.time.js"
                                      ,"~/admin/js/plugins/chart/chart.js"
                                      ,"~/admin/js/plugins/radiolist/radiolist.js"
                                      ,"~/admin/js/plugins/help/help.js"
                                       ,"~/admin/js/plugins/transformer/transformer.js"

                                      ,"~/admin/js/plugins/datepicker/bootstrap-datepicker.js"
                                      ,"~/admin/js/plugins/datepicker/locales/bootstrap-datepicker." + SettingsMain.Language.Split('-')[0] + ".js"

                                      // TODO: dublicate client side js (different versions)
                                      ,"~/admin/js/plugins/jqfileupload/vendor/jquery.ui.widget.js"
                                      ,"~/admin/js/plugins/jqfileupload/jquery.iframe-transport.js"
                                      ,"~/admin/js/plugins/jqfileupload/jquery.fileupload.js"

                                      ,"~/admin/js/plugins/noticeStatistic/noticeStatistic.js"
                                      ,"~/admin/js/masterpage/adminsearch.js"
                                      ,"~/admin/js/masterpage/saasIndicator.js"
                                      ,"~/admin/js/masterpage/ordersCount.js"
                                      ,"~/admin/js/masterpage/share.js"
                                      ,"~/admin/js/masterpage/ordersCount.js"
                                      ,"~/admin/js/masterpage/achievementsHelp.js"
                                      ,"~/admin/js/masterpage/achievementsPopup.js"
                                      ,"~/admin/js/masterpage/showcase.js"
            },
                                      "adminall.js");

        }

        protected void lnkExit_Click(object sender, EventArgs e)
        {
            AuthorizeService.SignOut();
            CustomerContext.IsDebug = false;
            Response.Redirect("~/");
        }
    }
}