//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Web;

namespace AdvantShop.Helpers
{
    public class BrowsersHelper
    {
        const string urlOldBrowsersPage = "~/oldBrowser/default.aspx";

        public static Dictionary<string, int> SupportedBrowsers = new Dictionary<string, int>() { 
            {"IE", 10},
        };

        public static bool IsSupportedBrowser()
        {
            HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;

          return SupportedBrowsers.ContainsKey(browser.Browser) ? SupportedBrowsers[browser.Browser] <= browser.MajorVersion : true;
        }

        public static void CheckSupportedBrowser()
        {
            HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;

            if (!IsSupportedBrowser())
            {
                RedirectToOldBrowsersPage();
            }
        }

        public static void RedirectToOldBrowsersPage()
        {
            HttpContext.Current.Response.Redirect(urlOldBrowsersPage, true);
        }

    }
}
