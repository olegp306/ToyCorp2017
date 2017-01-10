using System;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using System.Text;
using System.Linq;
using AdvantShop.Customers;

//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

public partial class UserControls_MenuTopAlternativeSocial : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        searchBlock.Visible = SettingsDesign.SearchBlockLocation == SettingsDesign.eSearchBlockLocation.CatalogMenu;
    }

    public string GetMenu()
    {
        string cacheName = "MenuTopSocialAlternative";
        if (CacheManager.Contains(cacheName))
        {
            return CacheManager.Get<string>(cacheName);
        }

        var result = new StringBuilder();
        int rootIndex = 0;
        foreach (var mItem in MenuService.GetEnabledChildMenuItemsByParentId(0, MenuService.EMenuType.Top, EMenuItemShowMode.NotAuthorized))
        {
            if (mItem.MenuItemUrlType == EMenuItemUrlType.StaticPage)
            {
                var page = StaticPageService.GetStaticPage(mItem.MenuItemUrlPath.Split('/').LastOrDefault());
                if (page != null)
                {
                    if (rootIndex != 0)
                    {
                        result.AppendFormat("<div class=\"tree-item-split\"></div>");
                        rootIndex++;
                    }

                    result.Append("<div class=\"tree-item\"><div class=\"tree-item-inside\">");

                    result.AppendFormat("<a href=\"{0}\" class=\"{1}\"{2}>{3}</a>",
                                    "social/staticpageviewsocial.aspx?staticpageid=" + page.ID,
                                    "tree-item-link",
                                    string.Empty,
                                    mItem.MenuItemName);

                    result.AppendFormat("</div></div>");
                }
            }
        }
        CacheManager.Insert(cacheName, result.ToString());
        return result.ToString();
    }
}