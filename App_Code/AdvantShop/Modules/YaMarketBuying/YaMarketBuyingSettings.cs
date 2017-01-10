namespace AdvantShop.Modules
{
    public class YaMarketBuyingSettings
    {
        private const string ModuleStringId = "YaMarketBuying";

        public static string AuthToken
        {
            get { return ModuleSettingsProvider.GetSettingValue<string>("AuthToken", ModuleStringId); }
            set { ModuleSettingsProvider.SetSettingValue("AuthToken", value, ModuleStringId); }
        }

        public static string Payments
        {
            get { return ModuleSettingsProvider.GetSettingValue<string>("Payments", ModuleStringId); }
            set { ModuleSettingsProvider.SetSettingValue("Payments", value, ModuleStringId); }
        }

        public static string Outlets
        {
            get { return ModuleSettingsProvider.GetSettingValue<string>("Outlets", ModuleStringId); }
            set { ModuleSettingsProvider.SetSettingValue("Outlets", value, ModuleStringId); }
        }

        public static int UpaidStatusId
        {
            get { return ModuleSettingsProvider.GetSettingValue<int>("UpaidStatusId", ModuleStringId); }
            set { ModuleSettingsProvider.SetSettingValue("UpaidStatusId", value, ModuleStringId); }
        }

        public static int ProcessingStatusId
        {
            get { return ModuleSettingsProvider.GetSettingValue<int>("ProcessingStatusId", ModuleStringId); }
            set { ModuleSettingsProvider.SetSettingValue("ProcessingStatusId", value, ModuleStringId); }
        }

        public static int DeliveryStatusId
        {
            get { return ModuleSettingsProvider.GetSettingValue<int>("DeliveryStatusId", ModuleStringId); }
            set { ModuleSettingsProvider.SetSettingValue("DeliveryStatusId", value, ModuleStringId); }
        }

        public static int DeliveredStatusId
        {
            get { return ModuleSettingsProvider.GetSettingValue<int>("DeliveredStatusId", ModuleStringId); }
            set { ModuleSettingsProvider.SetSettingValue("DeliveredStatusId", value, ModuleStringId); }
        }

        public static string CampaignId
        {
            get { return ModuleSettingsProvider.GetSettingValue<string>("CampaignId", ModuleStringId); }
            set { ModuleSettingsProvider.SetSettingValue("CampaignId", value, ModuleStringId); }
        }

        public static string AuthTokenToMarket
        {
            get { return ModuleSettingsProvider.GetSettingValue<string>("AuthTokenToMarket", ModuleStringId); }
            set { ModuleSettingsProvider.SetSettingValue("AuthTokenToMarket", value, ModuleStringId); }
        }

        public static string Login
        {
            get { return ModuleSettingsProvider.GetSettingValue<string>("Login", ModuleStringId); }
            set { ModuleSettingsProvider.SetSettingValue("Login", value, ModuleStringId); }
        }

        public static string AuthClientId
        {
            get { return ModuleSettingsProvider.GetSettingValue<string>("AuthClientId", ModuleStringId); }
            set { ModuleSettingsProvider.SetSettingValue("AuthClientId", value, ModuleStringId); }
        }
    }
}