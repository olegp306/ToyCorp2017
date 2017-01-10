//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Web;
using AdvantShop.Helpers;

namespace AdvantShop.Configuration
{
    public class SettingsMain
    {
        public static bool EnableInplace
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["EnableInplace"]); }
            set { SettingProvider.Items["EnableInplace"] = value.ToString(); }
        }

        public static bool EnableCaptcha
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["EnableCheckOrderConfirmCode"]); }
            set { SettingProvider.Items["EnableCheckOrderConfirmCode"] = value.ToString(); }
        }

        public static bool EnablePhoneMask
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["EnablePhoneMask"]); }
            set { SettingProvider.Items["EnablePhoneMask"] = value.ToString(); }
        }

        public static bool EnableAutoUpdateCurrencies
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["EnableAutoUpdateCurrencies"]); }
            set { SettingProvider.Items["EnableAutoUpdateCurrencies"] = value.ToString(); }
        }

        public static string LogoImageName
        {
            get { return SettingProvider.Items["MainPageLogoFileName"]; }
            set { SettingProvider.Items["MainPageLogoFileName"] = value; }
        }

        public static string FaviconImageName
        {
            get { return SettingProvider.Items["MainFaviconFileName"]; }
            set { SettingProvider.Items["MainFaviconFileName"] = value; }
        }

        public static string SiteUrl
        {
            get { return SettingProvider.Items["ShopURL"]; }
            set { SettingProvider.Items["ShopURL"] = value; }
        }

        public static string SiteUrlPlain
        {
            get { return SiteUrl.Replace("http://", "").Replace("https://", "").Replace("www.", ""); }
        }


        public static string ShopName
        {
            get { return SettingProvider.Items["ShopName"]; }
            set { SettingProvider.Items["ShopName"] = value; }
        }

        public static string LogoImageAlt
        {
            get { return SettingProvider.Items["ImageAltText"]; }
            set { SettingProvider.Items["ImageAltText"] = value; }
        }

        public static string Language
        {
            get { return SettingProvider.Items["Language"]; }
            set { SettingProvider.Items["Language"] = value; }
        }

        public static string AdminDateFormat
        {
            get { return SettingProvider.Items["AdminDateFormat"]; }
            set { SettingProvider.Items["AdminDateFormat"] = value; }
        }

        public static string ShortDateFormat
        {
            get { return SettingProvider.Items["ShortDateFormat"]; }
            set { SettingProvider.Items["ShortDateFormat"] = value; }
        }

        public static int SellerCountryId
        {
            get { return SQLDataHelper.GetInt(SettingProvider.Items["SellerCountryId"]); }
            set { SettingProvider.Items["SellerCountryId"] = value.ToString(); }
        }

        public static int SellerRegionId
        {
            get { return SQLDataHelper.GetInt(SettingProvider.Items["SellerRegionId"]); }
            set { SettingProvider.Items["SellerRegionId"] = value.ToString(); }
        }

        public static string Phone
        {
            get { return SettingProvider.Items["Phone"]; }
            set { SettingProvider.Items["Phone"] = value; }
        }

        public static string City
        {
            get { return SettingProvider.Items["City"]; }
            set { SettingProvider.Items["City"] = value; }
        }

        public static string SearchPage
        {
            get { return SettingProvider.Items["SearchPage"]; }
            set { SettingProvider.Items["SearchPage"] = value; }
        }

        public static string SearchArea
        {
            get { return SettingProvider.Items["SearchArea"]; }
            set { SettingProvider.Items["SearchArea"] = value; }
        }

        public static string Achievements
        {
            get { return SettingProvider.Items["Achievements"]; }
            set { SettingProvider.Items["Achievements"] = value; }
        }

        public static string AchievementsPoints
        {
            get { return SettingProvider.Items["AchievementsPoints"]; }
            set { SettingProvider.Items["AchievementsPoints"] = value; }
        }

        public static string AchievementsDescription
        {
            get { return SettingProvider.Items["AchievementsDescription"]; }
            set { SettingProvider.Items["AchievementsDescription"] = value; }
        }

        public static string AchievementsPopUp
        {
            get { return SettingProvider.Items["AchievementsPopUp"]; }
            set { SettingProvider.Items["AchievementsPopUp"] = value; }
        }

    }
}