//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web;
using AdvantShop.Helpers;

namespace AdvantShop.Configuration
{
    public class SettingsDesign
    {
        private const string TemplateContextKey = "CurrentTemplate";

        public enum eSearchBlockLocation
        {
            None = 0,
            TopMenu = 1,
            CatalogMenu = 2
        }
        
        public enum eMainPageMode
        {
            Default = 0,
            TwoColumns = 1,
            ThreeColumns = 2
        }

        public enum eShowShippingsInDetails
        {
            Never = 0,
            ByClick = 1,
            Always = 2
        }

        #region Design settings in db

        public static string Template
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    var currentTemplate = HttpContext.Current.Items[TemplateContextKey] as string;
                    if (currentTemplate != null) 
                        return currentTemplate;

                    HttpContext.Current.Items[TemplateContextKey] = SettingProvider.Items["Template"];
                }
                return SettingProvider.Items["Template"];
            }
            set
            {
                HttpContext.Current.Items[TemplateContextKey] = value;
            }
        }

        public static void ChangeTemplate(string template)
        {
            SettingProvider.Items["Template"] = template;
            Template = template;
        }

        public static bool ShoppingCartVisibility
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["ShowShoppingCartOnMainPage"]); }
            set { SettingProvider.Items["ShowShoppingCartOnMainPage"] = value.ToString(); }
        }

        public static bool EnableZoom
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["EnabledZoom"]); }
            set { SettingProvider.Items["EnabledZoom"] = value.ToString(); }
        }

        public static bool DisplayToolBarBottom
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["DisplayToolBarBottom"]); }
            set { SettingProvider.Items["DisplayToolBarBottom"] = value.ToString(); }
        }

        public static eShowShippingsInDetails ShowShippingsMethodsInDetails
        {
            get { return (eShowShippingsInDetails)int.Parse(SettingProvider.Items["ShowShippingsMethodsInDetails"]); }
            set { SettingProvider.Items["ShowShippingsMethodsInDetails"] = ((int)value).ToString(); }
        }

        public static int ShippingsMethodsInDetailsCount
        {
            get { return int.Parse(SettingProvider.Items["ShippingsMethodsInDetailsCount"]); }
            set { SettingProvider.Items["ShippingsMethodsInDetailsCount"] = value.ToString(); }
        }

        public static bool DisplayCityInTopPanel
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["DisplayCityInTopPanel"]); }
            set { SettingProvider.Items["DisplayCityInTopPanel"] = value.ToString(); }
        }

        public static bool DisplayCityBubble
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["DisplayCityBubble"]); }
            set { SettingProvider.Items["DisplayCityBubble"] = value.ToString(); }
        }
        
        #endregion

        #region Current template settings in template.config

        public static string Theme
        {
            get { return TemplateSettingsProvider.Items["Theme"]; }
            set { TemplateSettingsProvider.Items["Theme"] = value; }
        }

        public static string ColorScheme
        {
            get { return TemplateSettingsProvider.Items["ColorScheme"]; }
            set { TemplateSettingsProvider.Items["ColorScheme"] = value; }
        }

        public static string BackGround
        {
            get { return TemplateSettingsProvider.Items["BackGround"]; }
            set { TemplateSettingsProvider.Items["BackGround"] = value; }
        }

        public static eSearchBlockLocation SearchBlockLocation
        {
            get 
            {
                var result = eSearchBlockLocation.CatalogMenu;
                Enum.TryParse<eSearchBlockLocation>(TemplateSettingsProvider.Items["SearchBlockLocation"], out result);
                return result;
            }
            set { TemplateSettingsProvider.Items["SearchBlockLocation"] = value.ToString(); }
        }

        public static eMainPageMode MainPageMode
        {
            get 
            { 
                var mainPageMode = eMainPageMode.Default;
                Enum.TryParse<eMainPageMode>(TemplateSettingsProvider.Items["MainPageMode"], out mainPageMode);
                return mainPageMode;
            }
            set { TemplateSettingsProvider.Items["MainPageMode"] = value.ToString(); }
        }
        
        public static string CarouselAnimation
        {
            get { return TemplateSettingsProvider.Items["CarouselAnimation"]; }
            set { TemplateSettingsProvider.Items["CarouselAnimation"] = value; }
        }

        public static int CarouselAnimationSpeed
        {
            get
            {
                int intTempResult = -1;
                Int32.TryParse(TemplateSettingsProvider.Items["CarouselAnimationSpeed"], out intTempResult);
                return intTempResult;
            }
            set { TemplateSettingsProvider.Items["CarouselAnimationSpeed"] = value.ToString(); }
        }

        public static int CarouselAnimationDelay
        {
            get
            {
                int intTempResult = -1;
                Int32.TryParse(TemplateSettingsProvider.Items["CarouselAnimationDelay"], out intTempResult);
                return intTempResult;
            }
            set { TemplateSettingsProvider.Items["CarouselAnimationDelay"] = value.ToString(); }
        }        

        /// <summary>
        /// ShowSeeProductOnMainPage
        /// </summary>
        public static bool RecentlyViewVisibility
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["RecentlyViewVisibility"]); }
            set { TemplateSettingsProvider.Items["RecentlyViewVisibility"] = value.ToString(); }
        }
        
        /// <summary>
        /// ShowNewsOnMainPage
        /// </summary>
        public static bool NewsVisibility
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["NewsVisibility"]); }
            set { TemplateSettingsProvider.Items["NewsVisibility"] = value.ToString(); }
        }

        /// <summary>
        /// ShowNewsSubscriptionOnMainPage
        /// </summary>
        public static bool NewsSubscriptionVisibility
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["NewsSubscriptionVisibility"]); }
            set { TemplateSettingsProvider.Items["NewsSubscriptionVisibility"] = value.ToString(); }
        }

        /// <summary>
        /// ShowVotingOnMainPage
        /// </summary>
        public static bool VotingVisibility
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["VotingVisibility"]); }
            set { TemplateSettingsProvider.Items["VotingVisibility"] = value.ToString(); }
        }

        /// <summary>
        /// ShowStatusCommentOnMainPage
        /// </summary>
        public static bool CheckOrderVisibility
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["CheckOrderVisibility"]); }
            set { TemplateSettingsProvider.Items["CheckOrderVisibility"] = value.ToString(); }
        }

        /// <summary>
        /// ShowFilterInCatalog
        /// </summary>
        public static bool FilterVisibility
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["FilterVisibility"]); }
            set { TemplateSettingsProvider.Items["FilterVisibility"] = value.ToString(); }
        }
        
        /// <summary>
        /// ShowMainPageProductsOnMainPage
        /// </summary>
        public static bool MainPageProductsVisibility
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["MainPageProductsVisibility"]); }
            set { TemplateSettingsProvider.Items["MainPageProductsVisibility"] = value.ToString(); }
        }
        
        /// <summary>
        /// GiftSertificateBlock
        /// </summary>
        public static bool GiftSertificateVisibility
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["GiftSertificateVisibility"]); }
            set { TemplateSettingsProvider.Items["GiftSertificateVisibility"] = value.ToString(); }
        }

        /// <summary>
        /// WishList
        /// </summary>
        public static bool WishListVisibility
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["WishListVisibility"]); }
            set { TemplateSettingsProvider.Items["WishListVisibility"] = value.ToString(); }
        }

        /// <summary>
        /// CarouseltVisibility
        /// </summary>
        public static bool CarouselVisibility
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["CarouselVisibility"]); }
            set { TemplateSettingsProvider.Items["CarouselVisibility"] = value.ToString(); }
        }

        public static int CountProductInLine
        {
            get
            {
                if (Demo.IsDemoEnabled &&  CommonHelper.GetCookieString("ProductsCount").TryParseInt() != 0)
                {
                    return CommonHelper.GetCookieString("ProductsCount").TryParseInt();
                }
                else
                {
                    return TemplateSettingsProvider.Items["CountProductInLine"].TryParseInt();
                }
                
            }
            set
            {
                if (Demo.IsDemoEnabled)
                {
                    CommonHelper.SetCookie("ProductsCount", value.ToString());
                }
                else
                {
                    TemplateSettingsProvider.Items["CountProductInLine"] = value.ToString();    
                }
                
            }
        }        

        public static bool EnableSocialShareButtons
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["EnableSocialShareButtons"]); }
            set { TemplateSettingsProvider.Items["EnableSocialShareButtons"] = value.ToString(); }
        }

        #endregion;
    }
}