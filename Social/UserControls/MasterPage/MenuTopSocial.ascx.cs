using System;
using System.Linq;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.UrlRewriter;

//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

public partial class UserControls_MenuTop_Social : System.Web.UI.UserControl
{
    protected string GetHtml()
    {
        string cacheName = "MenuTopSocial";
        if (CacheManager.Contains(cacheName))
        {
            return CacheManager.Get<string>(cacheName);
        }

        string result = string.Empty;

        foreach (var mItem in MenuService.GetEnabledChildMenuItemsByParentId(0, MenuService.EMenuType.Top, EMenuItemShowMode.NotAuthorized))
        {
            if (mItem.MenuItemUrlType == EMenuItemUrlType.StaticPage)
            {
                var page = StaticPageService.GetStaticPage(mItem.MenuItemUrlPath.Split('/').LastOrDefault());
                if(page != null)
                {
                    result += string.Format("<a href=\"{0}\"{1}>{2}</a>",
                                        "social/staticpageviewsocial.aspx?staticpageid=" + page.ID,
                                        mItem.Blank ? "target=\"_blank\"" : string.Empty, mItem.MenuItemName);
                }
                
            }
        }
        CacheManager.Insert(cacheName, result);
        return result;
    }

}