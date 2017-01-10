//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.SEO;
using Resources;
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AdvantShop.Controls
{
    public class AdvantShopPage : Page
    {

        private const string AntiXsrfTokenKey = "__AntiXsrfToken";
        private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
        private string _antiXsrfTokenValue;

        protected override void InitializeCulture()
        {
            Localization.Culture.InitializeCulture();
            //Localization.Culture.InitializeCulture(AdvantshopConfigService.GetLocalization()); так сделано чтобы не долбиьтся в базу, а брать из конфига , НО тогда не работает переключение языков
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (AppServiceStartAction.state != DataBaseService.PingDbState.NoError)
            {
                SessionServices.StartSession(HttpContext.Current);
                return;
            }

            if (!File.Exists(SettingsGeneral.InstallFilePath))
            {
                Response.Redirect(UrlService.GetAbsoluteLink("install/default.aspx"));
            }

            AdvantShop.Helpers.BrowsersHelper.CheckSupportedBrowser();


            var requestCookie = Request.Cookies[AntiXsrfTokenKey];
            Guid requestCookieGuidValue;
            if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
            {
                // Use the Anti-XSRF token from the cookie
                _antiXsrfTokenValue = requestCookie.Value;
                Page.ViewStateUserKey = _antiXsrfTokenValue;
            }
            else
            {
                // Generate a new Anti-XSRF token and save to the cookie
                _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
                Page.ViewStateUserKey = _antiXsrfTokenValue;

                var responseCookie = new HttpCookie(AntiXsrfTokenKey)
                {
                    HttpOnly = true,
                    Value = _antiXsrfTokenValue
                };
                if (Request.IsSecureConnection)
                {
                    responseCookie.Secure = true;
                }
                Response.Cookies.Set(responseCookie);
            }
        }


        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            if (Page.EnableViewState)
            {
                if (!IsPostBack)
                {
                    // Set Anti-XSRF token
                    ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
                    ViewState[AntiXsrfUserNameKey] = CustomerContext.CustomerId.ToString();
                }
                else
                {
                    // Validate the Anti-XSRF token
                    if ((string) ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue ||
                        (string) ViewState[AntiXsrfUserNameKey] != CustomerContext.CustomerId.ToString())
                    {
                        throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
                    }
                }
            }
        }

        protected void Error404()
        {
            Server.Transfer(UrlService.GetAbsoluteLink("/err404.aspx"));
        }

        #region SetMeta

        protected void SetMeta(string pageTitle)
        {
            SetMetaTags(new MetaInfo(pageTitle));
        }

        protected MetaInfo SetMeta(MetaInfo meta, string name = null, string categoryName = null, string brandName = null, string price=null, int page = 0)
        {
            var newMeta = meta != null ? (MetaInfo)meta.Clone() : MetaInfoService.GetDefaultMetaInfo(); // Creating new object to modify - keeping original Meta for cache
            
            if (page > 1)
            {
                newMeta.Title += Resource.Client_Catalog_PageIs + page;
                newMeta.H1 += Resource.Client_Catalog_PageIs + page;
                newMeta.MetaDescription += Resource.Client_Catalog_PageIs + page;
            }

            SetMetaTags(MetaInfoService.GetFormatedMetaInfo(newMeta, name, categoryName, brandName, price));

            return newMeta;
        }

        private void SetMetaTags(MetaInfo meta)
        {
            var contr = (Literal)Page.Controls[0].FindControl("headMeta");
            if (contr == null)
                return;

            var sb = new StringBuilder();
            sb.AppendFormat("<title>{0}</title>\n", meta.Title);
            sb.AppendFormat("<meta name='Description' content='{0}'/>\n", meta.MetaDescription);
            sb.AppendFormat("<meta name='Keywords' content='{0}'/>\n", meta.MetaKeywords);
            contr.Text = sb.ToString();
        }
        #endregion
    }
}