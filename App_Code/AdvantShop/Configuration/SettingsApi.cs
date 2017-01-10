//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Configuration
{
    public class SettingsApi
    {
        public static string ApiKey
        {
            get { return SettingProvider.Items["Api_ApiKey"]; }
            set { SettingProvider.Items["Api_ApiKey"] = value; }
        }
    }
}