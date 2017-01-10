//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Caching;
using AdvantShop.Configuration;


namespace AdvantShop.Helpers
{
    public class MobileHelper
    {

        private static readonly string[] Mobiles = 
                {
                    "midp", "j2me", "avant", "docomo", 
                    "novarra", "palmos", "palmsource", 
                    "240x320", "opwv", "chtml",
                    "pda", "windows ce", "mmp/", 
                    "blackberry", "mib/", "symbian", 
                    "wireless", "nokia", "hand", "mobi",
                    "phone", "cdm", "up.b", "audio", 
                    "sie-", "sec-", "samsung", "HTC", 
                    "mot-", "mitsu", "sagem", "sony",
                    "alcatel", "lg", "eric", "vx", 
                    "nec", "philips", "mmm", "xx", 
                    "panasonic", "sharp", "wap", "sch",
                    "rover", "pocket", "benq", 
                    "pt", "pg", "vox", "amoi", 
                    "bird", "compal", "kg", "voda",
                    "sany", "kdd", "dbt", "sendo", 
                    "sgh", "gradi", "jb", "dddi", 
                    "moto", "iphone" , "mobile", "ipad"
                };

        public static bool IsMobileEnabled()
        {
            return (IsMobileBrowser() && ExistMobileTemplate() && !IsDesktopForced()) || IsMobileByUrl();
        }

        public static void RedirectToMobile(HttpContext context)
        {
            if (((IsMobileBrowser() && ExistMobileTemplate() && !IsDesktopForced() && !IsMobileByUrl())
                || (context.Items["RedirectToMobile"] != null && context.Items["RedirectToMobile"].ToString() == "true")) 
                && !CommonHelper.isLocalUrl())
            {
                context.Items.Remove("RedirectToMobile");
                context.Response.Redirect("http://m." + (CommonHelper.GetParentDomain() + context.Request.RawUrl));

            }
        }

        public static void RedirectToDesktop(HttpContext context)
        {
            SetDesktopForcedCookie();
            context.Response.Redirect("http://" + CommonHelper.GetParentDomain() + context.Request.RawUrl);
        }

        public static bool ExistMobileTemplate()
        {
            const string filePathToMobile = "~/Templates/Mobile";
            //return Directory.Exists(HttpContext.Current.Server.MapPath(filePathToMobile));
            return false;
        }

        /// <summary>
        /// Detect mobile browsers by USER_AGENT context
        /// </summary>
        public static bool IsMobileBrowser()
        {
            HttpContext context = HttpContext.Current;

            //default check
            if (context.Request.Browser.IsMobileDevice)
            {
                return true;
            }

            if (context.Request.ServerVariables["HTTP_USER_AGENT"].IsNullOrEmpty())
                return false;

            string currnetUserAgent = context.Request.ServerVariables["HTTP_USER_AGENT"].ToLower();
            return Mobiles.Any(currnetUserAgent.Contains);
        }


        /// <summary>
        /// Go to mobile version forced by domain
        /// </summary>
        public static bool IsMobileByUrl()
        {
            HttpContext context = HttpContext.Current;
            return context.Request.Url.AbsoluteUri.Contains("http://m.") || context.Request.Url.AbsoluteUri.Contains("https://m.");
        }

        /// <summary>
        /// Checks the necessity of transition to the desktop version by cookie
        /// </summary>
        public static bool IsDesktopForced()
        {
            var isDesktopForced = CommonHelper.GetCookie("ForcedDesktop");
            return isDesktopForced != null && isDesktopForced.Value == "true";
        }

        public static void SetDesktopForcedCookie()
        {
            CommonHelper.SetCookie("ForcedDesktop", "true", true);
        }

        public static void DeleteDesktopForcedCookie()
        {
            HttpContext.Current.Items.Add("RedirectToMobile", "true");
            CommonHelper.DeleteCookie("ForcedDesktop");
        }

    }
}