using System;
using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Core;

namespace Admin
{
    public partial class m_MasterPage : System.Web.UI.MasterPage
    {
        public void Page_PreRender(object sender, EventArgs e)
        {
            lBase.Text = string.Format("<base href='{0}'/>",
                           Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath +
                           (!Request.ApplicationPath.EndsWith("/") ? "/" : string.Empty)
                           + "admin/");

            JsCssTool.ReCreateIfNotExist();

            headStyle.Text = JsCssTool.MiniCss(new List<string>
                {
                    "~/admin/css/new_admin/buttons.css",
                    "~/admin/css/jquery.tooltip.css",
                    "~/admin/css/AdminStyle.css",
                    "~/admin/css/catalogDataTreeStyles.css",
                    "~/admin/css/exportFeedStyles.css",
                    "~/admin/css/jqueryslidemenu.css",
                    "~/css/jq/jquery.autocomplete.css",
                    "~/css/advcss/modal.css",
                    "~/admin/js/plugins/datepicker/css/datepicker.css"
                },
                "madmincss.css");

            headScript.Text = JsCssTool.MiniJs(new List<string>
            {
                "~/js/jq/jquery-1.7.1.min.js",
                "~/js/jq/jquery.autocomplete.js",
                "~/js/jq/jquery.metadata.js",
                "~/js/advjs/advModal.js",
                "~/js/advjs/advTabs.js",
                "~/js/advjs/advUtils.js",
                "~/admin/js/jquery.cookie.min.js",
                "~/admin/js/jquery.qtip.min.js",
                "~/admin/js/jquery.tooltip.min.js",
                "~/admin/js/slimbox2.js",
                "~/admin/js/jquery.history.js",
                "~/admin/js/jquerytimer.js",
                "~/admin/js/admin.js",
                "~/admin/js/grid.js",
                "~/admin/js/plugins/datepicker/bootstrap-datepicker.js",
                "~/admin/js/plugins/datepicker/locales/bootstrap-datepicker." + SettingsMain.Language.Split('-')[0] + ".js"
            },
            "madmin.js");
        }
    }
}
