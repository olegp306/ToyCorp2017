//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.SEO;

namespace AdvantShop.Configuration
{
    public class SettingsNews
    {
        public static int NewsMainPageCount
        {
            get { return int.Parse(SettingProvider.Items["NewsMainPageCount"]); }
            set { SettingProvider.Items["NewsMainPageCount"] = value.ToString(); }
        }

        public static int NewsPerPage
        {
            get { return int.Parse(SettingProvider.Items["NewsPerPage"]); }
            set { SettingProvider.Items["NewsPerPage"] = value.ToString(); }
        }

        public static string NewsMetaTitle
        {
            get { return SettingProvider.Items[MetaType.News + "Title"]; }
            set { SettingProvider.Items[MetaType.News + "Title"] = value; }
        }

        public static string NewsMetaDescription
        {
            get { return SettingProvider.Items[MetaType.News + "MetaDescription"]; }
            set { SettingProvider.Items[MetaType.News + "MetaDescription"] = value; }
        }

        public static string NewsMetaKeywords
        {
            get { return SettingProvider.Items[MetaType.News + "MetaKeywords"]; }
            set { SettingProvider.Items[MetaType.News + "MetaKeywords"] = value; }
        }

        public static string NewsMetaH1
        {
            get { return SettingProvider.Items[MetaType.News + "H1"]; }
            set { SettingProvider.Items[MetaType.News + "H1"] = value; }
        }
    }
}