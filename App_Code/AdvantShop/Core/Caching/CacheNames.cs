//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Core.Caching
{
    /// <summary>
    /// Retun the special formated cache object names
    /// </summary>
    /// <remarks></remarks>
    public class CacheNames
    {
        private const string TemplateSetPref = "TemplateSettings_";
        //private const string CommonSetPref = "CommonSettings_";
        private const string Category = "Category_";
        private const string MainMenu = "MainMenu_";
        private const string MainMenuAuth = "MainMenuAuth_";
        private const string StaticBlock = "StaticBlock_";

        //public static string GetCommonSettingsCacheObjectName(string strName)
        //{
        //    return CommonSetPref + strName;
        //}

        public static string GetModuleSettingsCacheObjectName()
        {
            return "ModuleSettings";
        }

        public static string GetTemplateSettingsCacheObjectName(string template, string strName)
        {
            return TemplateSetPref + template + "_" + strName;
        }

        public static string GetCategoryCacheObjectPrefix()
        {
            return Category;
        }

        public static string GetCategoryCacheObjectName(int id)
        {
            return Category + id;
        }

        public static string GetMainMenuCacheObjectName(int id)
        {
            return MainMenu + id;
        }

        public static string GetStaticBlockCacheObjectName(string strName)
        {
            return StaticBlock + strName;
        }

        public static string GetMainMenuCacheObjectName()
        {
            return MainMenu;
        }

         public static string GetMainMenuAuthCacheObjectName()
         {
             return MainMenuAuth;
         }

        public static string GetBestSellersCacheObjectName()
        {
            return "GetBestSellers";
        }

        public static string GetBottomMenuCacheObjectName()
        {
            return "BottomMenu";
        }

        public static string GetBottomMenuAuthCacheObjectName()
        {
            return "BottomMenuAuth";
        }

        public static string GetOrderPriceDiscountCacheObjectName()
        {
            return "OrderPriceDiscount";
        }

        public static string GetXmlSettingsCacheObjectName()
        {
            return "XMLSettings";
        }

        public static string GetRoutesCacheObjectName()
        {
            return "Routes";
        }
        public static string GetNewsForMainPage()
        {
            return "NewsForMainPage";
        }

        public static string GetUrlCacheObjectName()
        {
            return "UrlSynonyms";
        }

        public static string GetAltSessionCacheObjectName(string sessionId)
        {
            return "AltSession_" + sessionId;
        }

        internal static string GetCurrenciesCacheObjectName()
        {
            return "Currencies";
        }

        public static string GetRoleActionsCacheObjectName(string customerId)
        {
            return "RoleActions_" + customerId;
        }

        public static string GetDesignCacheObjectName(string designType)
        {
            return "Designs_" + designType;
        }

        //public static string GetCustomerCacheObjectName(string customerId)
        //{
        //    return "Customer_" + customerId;
        //}
    }
}