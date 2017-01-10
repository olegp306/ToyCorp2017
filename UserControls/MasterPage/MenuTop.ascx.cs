//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.CMS;
using AdvantShop.Core.Caching;
using AdvantShop.Customers;

namespace UserControls.MasterPage
{
    public partial class MenuTop : System.Web.UI.UserControl
    {
        protected string GetHtml()
        {
            var isRegistered = CustomerContext.CurrentCustomer.RegistredUser;
            var cacheName = !isRegistered
                ? CacheNames.GetMainMenuCacheObjectName()
                : CacheNames.GetMainMenuAuthCacheObjectName();

            if (CacheManager.Contains(cacheName))
                return CacheManager.Get<string>(cacheName);

            string result = string.Empty;

            foreach (
                var mItem in
                    MenuService.GetEnabledChildMenuItemsByParentId(0, MenuService.EMenuType.Top,
                        isRegistered ? EMenuItemShowMode.Authorized : EMenuItemShowMode.NotAuthorized))
            {
                if (mItem.NoFollow)
                {
                    result += "<!--noindex-->";
                }

                result += string.Format("<a href=\"{0}\"{1}{3}>{2}</a>\n",
                    mItem.MenuItemUrlPath,
                    mItem.Blank ? " target=\"_blank\"" : string.Empty,
                    mItem.MenuItemName,
                    mItem.NoFollow ? " rel='nofollow'" : string.Empty);

                if (mItem.NoFollow)
                {
                    result += "<!--/noindex-->";
                }
            }
            
            CacheManager.Insert(cacheName, result);
            return result;
        }
    }
}