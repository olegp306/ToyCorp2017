//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.Trial;

namespace AdvantShop.Core.UrlRewriter
{
    public class HttpUrlRewrite : IHttpModule
    {
        #region IHttpModule Members

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
        }

        #endregion
       
        private static void OnBeginRequest(object sender, EventArgs e)
        {

            var app = (HttpApplication)sender;
            string strCurrentUrl = app.Request.RawUrl.ToLower().Trim();
            app.StaticFile304();
            // Debug go first
            if (UrlService.IsDebugUrl(strCurrentUrl))
            {
                // Nothing here
                // just return
                return;
            }

            // Check cn
            if (AppServiceStartAction.state != DataBaseService.PingDbState.NoError)
            {
                // Nothing here
                // just return
                return;
            }

            // Check original pictures
            if (strCurrentUrl.Contains("/pictures/product/original/"))
            {
                app.Context.RewritePath("~/err404.aspx");
                return;
            }

            // Check price_temp folder
            if (strCurrentUrl.Contains("/price_temp/"))
            {
                var actions = RoleActionService.GetCustomerRoleActionsByCustomerId(CustomerContext.CurrentCustomer.Id);

                if (!(CustomerContext.CurrentCustomer.IsAdmin || TrialService.IsTrialEnabled ||
                    CustomerContext.CurrentCustomer.IsVirtual ||
                   (CustomerContext.CurrentCustomer.IsModerator && actions.Any(item => item.Key == RoleActionKey.DisplayOrders || item.Key == RoleActionKey.DisplayImportExport)))
                    )
                {
                    app.Context.RewritePath("~/err404.aspx");
                    return;
                }
            }

            // Social 
            string social = UrlService.Social.Find(strCurrentUrl.Contains);
            if (social != null)
            {
                app.Response.RedirectPermanent("~/social/catalogsocial.aspx?type=" + social.Split('-').Last());
            }

            // Check exportfeed
            //if (strCurrentUrl.Contains("exportfeed.aspx") || strCurrentUrl.Contains("exportfeeddet.aspx"))
            //    return;

            // Payment return url
            if (strCurrentUrl.Contains("/paymentreturnurl/"))
            {
                app.Context.RewritePath("~/PaymentReturnUrl.aspx?PaymentMethodID=" + app.Request.Path.Split(new[] { "/paymentreturnurl/" }, StringSplitOptions.None).LastOrDefault()
                                        + (string.IsNullOrWhiteSpace(app.Request.Url.Query) ? string.Empty : "&" + app.Request.Url.Query.Trim('?')));
                return;
            }
            if (strCurrentUrl.Contains("/paymentnotification/"))
            {
                app.Context.RewritePath("~/HttpHandlers/PaymentNotification.ashx?PaymentMethodID=" + app.Request.Path.Split(new[] { "/paymentnotification/" }, StringSplitOptions.None).LastOrDefault()
                    + (string.IsNullOrWhiteSpace(app.Request.Url.Query) ? string.Empty : "&" + app.Request.Url.Query.Trim('?')));
                return;
            }

            // Seek in url table
            foreach (var key in UrlService.UrlTable.Keys.Where(strCurrentUrl.Split('?')[0].EndsWith))
            {
                app.Context.RewritePath(UrlService.UrlTable[key] + (string.IsNullOrWhiteSpace(app.Request.Url.Query)
                                                                            ? string.Empty
                                                                            : (UrlService.UrlTable[key].Contains("?") ? "&" : "?") + app.Request.Url.Query.Trim('?')));
                return;
            }

            //// Storage
            //string storage = UrlService.Storages.Find(strCurrentUrl.Contains);
            //if (storage != null)
            //{
            //    var index = strCurrentUrl.IndexOf(storage, StringComparison.Ordinal);
            //    string tail = app.Request.RawUrl.Substring(index + storage.Length);
            //    string pathNew = string.Format("~{0}{1}", storage, tail);
            //    app.Context.RewritePath(pathNew);
            //    return;
            //}

            string path = strCurrentUrl;
            if (app.Request.ApplicationPath != "/")
            {
                if (app.Request.ApplicationPath != null)
                    path = path.Replace(app.Request.ApplicationPath.ToLower(), "");
            }

            // sometimes Path.GetExtension thows exeption "Illegal characters in path"
            try
            {
                string extention = Path.GetExtension(path.Split('?')[0]);
                if (UrlService.ExtentionNotToRedirect.Contains(extention))
                    return;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex, false);
            }

            //301 redirect if need
            if (SettingsSEO.Enabled301Redirects && !path.Contains("/admin/"))
            {
                string newUrl = UrlService.GetRedirect301(path.TrimStart('/').Trim('?'), app.Request.Url.AbsoluteUri);
                if (newUrl.IsNotEmpty())
                {
                    app.Response.RedirectPermanent(newUrl);
                    return;
                }
            }

            var modules = AttachedModules.GetModules<IModuleUrlRewrite>();

            foreach (var moduleType in modules)
            {
                var moduleObject = (IModuleUrlRewrite)Activator.CreateInstance(moduleType, null);
                string newUrl = path;
                if (moduleObject.RewritePath(path, ref newUrl))
                {
                    app.Context.RewritePath(newUrl);
                    return;
                }
            }

            var param = UrlService.ParseRequest(path);
            if (param != null)
            {
                UrlService.RedirectTo(app, param);
            }
            else if (path.IsNotEmpty() && path != "/" && !path.Contains(".") && !path.Contains("?"))
            {
                Debug.LogError(new HttpException(404, "Can't get url: " + app.Context.Request.RawUrl + "path: '" + path + "'"));
                app.Context.RewritePath("~/err404.aspx");
            }
        }
    }

    public static class UrlRewriteExt
    {
        public static void StaticFile304(this HttpApplication app)
        {
            var fileName = app.Request.PhysicalPath;
            if (!fileName.EndsWith(".xml")) return;

            var lastModified = File.GetLastWriteTime(fileName);
            var ifModifiedSince = app.Request.Headers["If-Modified-Since"].TryParseDateTimeGMT();
            
            if (!ifModifiedSince.HasValue && !string.IsNullOrWhiteSpace(app.Request.Headers["If-Modified-Since"]))
            {
                app.Request.Headers.Remove("If-Modified-Since");
            }
            if (ifModifiedSince.HasValue && (ifModifiedSince.Value >= lastModified))
            {
                app.Response.StatusCode = 304;
                app.Response.SuppressContent = true;
                return;
            }
            app.Response.Cache.SetLastModified(lastModified);
            //app.Response.AddHeader("Last-Modified", lastModified.ToUniversalTime().ToString("R"));
        }

        public static DateTime? TryParseDateTimeGMT(this string val)
        {
            DateTime intval;
            if (DateTime.TryParseExact(val, "R", CultureInfo.InvariantCulture, DateTimeStyles.None, out intval))
            {
                return intval.ToLocalTime();
            }
            return null;
        }
    }
}