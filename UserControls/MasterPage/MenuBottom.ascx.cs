//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Text;
using AdvantShop.CMS;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;

namespace UserControls.MasterPage
{
    public partial class MenuBottom : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CustomerContext.CurrentCustomer.RegistredUser)
            {
                var cacheName = CacheNames.GetBottomMenuCacheObjectName();
                if (CacheManager.Contains(cacheName))
                    ltbottomMenu.Text = CacheManager.Get<string>(cacheName);
                else
                {
                    ltbottomMenu.Text = GetHtml();
                    CacheManager.Insert<string>(cacheName, ltbottomMenu.Text);
                }
            }
            else
            {
                var cacheName = CacheNames.GetBottomMenuAuthCacheObjectName();
                if (CacheManager.Contains(cacheName))
                    ltbottomMenu.Text = CacheManager.Get<string>(cacheName);
                else
                {
                    ltbottomMenu.Text = GetHtml();
                    CacheManager.Insert<string>(cacheName, ltbottomMenu.Text);
                }
            }
        }

        private string GetHtml()
        {
            var result = new StringBuilder();
            if (SettingsCatalog.DisplayCategoriesInBottomMenu)
            {
                result.Append("<div class=\"block-footer\">");
                result.AppendFormat("<div class=\"block-title\">{0}</div>", Resources.Resource.Client_MasterPage_BottomMenu);

                result.Append("<menu class=\"block-content\">");
                foreach (
                    var cat in
                        CategoryService.GetChildCategoriesByCategoryId(0, false)
                                       .Where(cat => cat.Enabled && cat.ParentsEnabled))
                {
                    result.AppendFormat("<li class=\"menu-bottom-row\"><a href=\"{0}\" class=\"link-footer\">{1}</a></li>",
                                        UrlService.GetLink(ParamType.Category, cat.UrlPath, cat.CategoryId),
                                        cat.Name);
                }
                result.Append("</menu>");
                result.Append("</div>");
            }

            foreach (var rootMenuItem in MenuService.GetChildMenuItemsByParentId(0, MenuService.EMenuType.Bottom, CustomerContext.CurrentCustomer.RegistredUser ? EMenuItemShowMode.Authorized : EMenuItemShowMode.NotAuthorized))
            {
                if (!rootMenuItem.Enabled)
                    continue;
                result.Append("<div class=\"block-footer\">");
                result.AppendFormat("<div class=\"block-title\">{0}</div>", rootMenuItem.MenuItemName);

                result.Append("<menu class=\"block-content\">");

                foreach (var childMenuItem in MenuService.GetChildMenuItemsByParentId(rootMenuItem.MenuItemID, MenuService.EMenuType.Bottom, CustomerContext.CurrentCustomer.RegistredUser ? EMenuItemShowMode.Authorized : EMenuItemShowMode.NotAuthorized))
                {
                    result.Append(RenderChildItem(childMenuItem));
                }
                result.Append("</menu>");
                result.Append("</div>");
            }
            return result.ToString();
        }

        private string RenderChildItem(AdvMenuItem childMenuItem)
        {
            var result = new StringBuilder();
            result.Append("<li class=\"menu-bottom-row\">");
            if (!string.IsNullOrEmpty(childMenuItem.MenuItemIcon))
            {
                result.AppendFormat("<img src=\"{0}\" alt=\"\" class=\"menu-bottom-icon\" />", "pictures/icons/" + childMenuItem.MenuItemIcon);
            }
        if (childMenuItem.NoFollow)
        {
            result.Append("<!--noindex-->");
        }

        result.AppendFormat("<a href='{0}' class='link-footer'{2}{3}>{1}</a>",
                            childMenuItem.MenuItemUrlPath,
                            childMenuItem.MenuItemName,
                            childMenuItem.Blank ? " target='_blank'" : string.Empty,
                            childMenuItem.NoFollow ? " rel='nofollow'" : string.Empty);

        if (childMenuItem.NoFollow)
        {
            result.Append("<!--/noindex-->");
        }
            result.Append("</li>");
            return result.ToString();
        }
    }
}