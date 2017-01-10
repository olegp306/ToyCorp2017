//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web;
using AdvantShop.Core;
using AdvantShop.Helpers;
using AdvantShop.Security;
using AdvantShop.Trial;

namespace AdvantShop.Controls
{
    public class AdvantShopAdminPage : AdvantShopPage
    {
        protected override void OnInit(EventArgs e)
        {

            if (AppServiceStartAction.state != DataBaseService.PingDbState.NoError)
            {
                SessionServices.StartSession(HttpContext.Current);
                return;
            }

            base.OnInit(e);
            Secure.VerifySessionForErrors();
            Secure.VerifyAccessLevel();
            CommonHelper.DisableBrowserCache();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!Request.Browser.Crawler && CommonHelper.GetCookieString(TrialEvents.VisitAdminSide.ToString()).IsNullOrEmpty())
            {
                TrialService.TrackEvent(TrialEvents.VisitAdminSide, "IP:" + Request.UserHostAddress + " User-Agent: " + Request.UserAgent);
                CommonHelper.SetCookie(TrialEvents.VisitAdminSide.ToString(), DateTime.Now.ToString(), new TimeSpan(0, 20, 0), true);
            }
        }

    }
}