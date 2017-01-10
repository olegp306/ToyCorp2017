//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using System.Text;
using System.Linq;
using AdvantShop.Customers;

namespace UserControls.MasterPage
{
    public partial class MenuTopAlternative : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            searchBlock.Visible = SettingsDesign.SearchBlockLocation == SettingsDesign.eSearchBlockLocation.CatalogMenu;
        }

        public string GetMenu()
        {
            var useCache = !Request.Url.AbsolutePath.Contains("err404.aspx");

            var rawUrl = Request.RawUrl;
            var cachename = CacheNames.GetMainMenuCacheObjectName() + "Alternative_" + rawUrl;

            if (useCache && !CustomerContext.CurrentCustomer.RegistredUser &&
                CacheManager.Contains(cachename))
            {
                return CacheManager.Get<string>(cachename);
            }
            if (useCache && CustomerContext.CurrentCustomer.RegistredUser &&
                CacheManager.Contains(cachename))
            {
                return CacheManager.Get<string>(cachename);
            }

            var result = new StringBuilder();

            var rootCategories =
                MenuService.GetEnabledChildMenuItemsByParentId(0, MenuService.EMenuType.Top,
                    CustomerContext.CurrentCustomer.RegistredUser
                        ? EMenuItemShowMode.Authorized
                        : EMenuItemShowMode.NotAuthorized).ToList();

            for (int rootIndex = 0; rootIndex < rootCategories.Count; ++rootIndex)
            {
                result.AppendFormat("<div class=\"{0}\"><div class=\"tree-item-inside\">",
                    rawUrl.EndsWith(rootCategories[rootIndex].MenuItemUrlPath) ? "tree-item-selected" : "tree-item");

                if (rootCategories[rootIndex].NoFollow)
                {
                    result.Append("<!--noindex-->");
                }

                result.AppendFormat("<a href='{0}' class='{1}'{2}{4}>{3}</a>",
                    rootCategories[rootIndex].MenuItemUrlPath,
                    rootCategories[rootIndex].HasChild ? "tree-item-link tree-parent" : "tree-item-link",
                    rootCategories[rootIndex].Blank ? " target='_blank'" : string.Empty,
                    rootCategories[rootIndex].MenuItemName,
                    rootCategories[rootIndex].NoFollow ? " rel='nofollow'" : string.Empty);

                if (rootCategories[rootIndex].NoFollow)
                {
                    result.Append("<!--/noindex-->");
                }

                if (rootCategories[rootIndex].HasChild)
                {
                    result.AppendFormat("<div class=\"tree-submenu\">\r\n");
                    result.Append("<div class=\"tree-submenu-category\">\r\n<div class=\"tree-submenu-column\">");

                    foreach (
                        var children in
                            MenuService.GetEnabledChildMenuItemsByParentId(rootCategories[rootIndex].MenuItemID,
                                MenuService.EMenuType.Top,
                                CustomerContext.CurrentCustomer.RegistredUser
                                    ? EMenuItemShowMode.Authorized
                                    : EMenuItemShowMode.NotAuthorized))
                    {
                        if (children.NoFollow)
                        {
                            result.Append("<!--noindex-->");
                        }
                        result.AppendFormat("<a href='{0}'{1}{3}>{2}</a>",
                            children.MenuItemUrlPath,
                            children.Blank ? " target='_blank'" : string.Empty,
                            children.MenuItemName,
                            children.NoFollow ? " rel='nofollow'" : string.Empty);
                        if (children.NoFollow)
                        {
                            result.Append("<!--/noindex-->");
                        }

                    }
                    result.Append("</div></div>\r\n");
                    result.AppendFormat("</div>");
                }

                //Пункт в главном меню закрывается
                result.AppendFormat("</div></div>");

                if (rootIndex != rootCategories.Count - 1)
                {
                    result.AppendFormat("<div class=\"tree-item-split\"></div>");
                }
            }

            string resultstring = result.ToString();

            if (useCache && !CustomerContext.CurrentCustomer.RegistredUser)
                CacheManager.Insert(cachename, resultstring);
            else if (useCache && CustomerContext.CurrentCustomer.RegistredUser)
                CacheManager.Insert(cachename, resultstring);

            return resultstring;
        }
    }
}