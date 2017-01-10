//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.IO;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.SaasData;
using AdvantShop.Design;
using AdvantShop.Helpers;
using AdvantShop.Trial;

namespace AdvantShop.Controls
{
    public class AdvantShopClientPage : AdvantShopPage
    {
        public void ShowMessage(Notify.NotifyType notifyType, string message)
        {
            var masterPage = (AdvantShopMasterPage) (Master);
            if (masterPage != null) masterPage.ShowMessage(notifyType, message);
        }

        protected override void OnPreInit(EventArgs e)
        {
            if (AppServiceStartAction.state != DataBaseService.PingDbState.NoError)
            {
                SessionServices.StartSession(HttpContext.Current);
                return;
            }

            if (MobileHelper.IsMobileEnabled())
            {
               MasterPageFile =  VirtualPathUtility.ToAbsolute(("~/Templates/Mobile/MasterPage.master"));
            }
            else
            {
                if (SettingsDesign.Template != TemplateService.DefaultTemplateId
                    && File.Exists(Server.MapPath("~/Templates/" + SettingsDesign.Template + "/MasterPage.master")) &&
                    !Request.RawUrl.Contains("social")
                    && MasterPageFile != null)
                {
                    MasterPageFile = VirtualPathUtility.ToAbsolute(("~/Templates/")) + SettingsDesign.Template +
                                     "/MasterPage.master";
                }
            }

            base.OnPreInit(e);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.IsCorrect)
            {
                Response.Redirect(UrlService.GetAbsoluteLink("liccheck.aspx"));
            }

            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.IsWorkingNow)
            {
                Response.Redirect(UrlService.GetAbsoluteLink("/app_offline.html"));
            }

            if (CustomerContext.CurrentCustomer.IsVirtual || Demo.IsDemoEnabled || TrialService.IsTrialEnabled ||
                SaasDataService.IsSaasEnabled)
            {
                return;
            }

            if (!SettingsLic.ActiveLic && !Request.CurrentExecutionFilePath.Contains("err404.aspx") &&
                !Request.CurrentExecutionFilePath.Contains("adv-admin.aspx"))
            {
                Response.Redirect(UrlService.GetAbsoluteLink("liccheck.aspx"));
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            Response.Cache.SetExpires(DateTime.Now.AddSeconds(1));
            Response.Cache.SetMaxAge(new TimeSpan(0, 0, 0, 1));
            Response.CacheControl = "private";

            // temporary do not track client side

            //if (!Request.Browser.Crawler && CommonHelper.GetCookieString(TrialEvents.VisitClientSide.ToString()).IsNullOrEmpty())
            //{
            //    TrialService.TrackEvent(TrialEvents.VisitClientSide, "IP:" + Request.UserHostAddress + " User-Agent: " + Request.UserAgent);
            //    CommonHelper.SetCookie(TrialEvents.VisitClientSide.ToString(), DateTime.Now.ToString(), new TimeSpan(0, 20, 0), true);
            //}
        }
    }
}