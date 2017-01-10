//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Globalization;
using AdvantShop.Helpers;

namespace AdvantShop.Configuration
{
    public class SettingsLic
    {
        public static string LicKey
        {
            get { return SettingProvider.Items["LicKey"]; }
            set { SettingProvider.Items["LicKey"] = value; }
        }

        public static bool ActiveLic
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["ActiveLic"]); }
            set { SettingProvider.Items["ActiveLic"] = value.ToString(CultureInfo.InvariantCulture); }
        }
    }
}