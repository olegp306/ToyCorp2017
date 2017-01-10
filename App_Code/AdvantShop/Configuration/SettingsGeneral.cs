//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web;

namespace AdvantShop.Configuration
{
    public class SettingsGeneral
    {
        private static readonly Object ThisLock = new Object();
        private static string _absoluteUrl;
        public static string AbsoluteUrl { get { return _absoluteUrl; } }

        public static void SetAbsoluteUrl(string st)
        {
            lock (ThisLock)
            {
                _absoluteUrl = st;
            }
        }

        private static string _absolutePath;

        public static string AbsolutePath
        {
            get
            {
                if (_absolutePath.IsNullOrEmpty() && HttpContext.Current != null)
                    SetAbsolutePath(HttpContext.Current.Server.MapPath("~/"));
                return _absolutePath;
            }
        }

        public static void SetAbsolutePath(string st)
        {
            lock (ThisLock)
            {
                _absolutePath = st;
            }
        }

        public static string InstallFilePath { get { return AbsolutePath + "App_Data/install.txt"; } }

        public static string SiteVersion
        {
            get { return SettingProvider.GetConfigSettingValue("PublicVersion"); }
        }

        public static string SiteVersionDev
        {
            get { return SettingProvider.GetConfigSettingValue("Version"); }
        }

        public static string CurrentSaasId
        {
            get { return SettingProvider.Items["LicKey"]; }
            set { SettingProvider.Items["LicKey"] = value; }
        }

        public static string CsvSeparator
        {
            get { return SettingProvider.Items["CsvSeparator"]; }
            set { SettingProvider.Items["CsvSeparator"] = value; }
        }

        public static string CsvEnconing
        {
            get { return SettingProvider.Items["CsvEnconing"]; }
            set { SettingProvider.Items["CsvEnconing"] = value; }
        }

        public static string CsvColumSeparator
        {
            get { return SettingProvider.Items["CsvColumSeparator"]; }
            set { SettingProvider.Items["CsvColumSeparator"] = value; }
        }

        public static string CsvPropertySeparator
        {
            get { return SettingProvider.Items["CsvPropertySeparator"]; }
            set { SettingProvider.Items["CsvPropertySeparator"] = value; }
        }

        public static bool CsvExportNoInCategory
        {
            get { return SettingProvider.Items["CsvExportNoInCategory"].TryParseBool(); }
            set { SettingProvider.Items["CsvExportNoInCategory"] = value.ToString(); }
        }
    }
}

