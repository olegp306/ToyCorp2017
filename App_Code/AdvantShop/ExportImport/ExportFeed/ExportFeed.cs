//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Data;
using System.Data.SqlClient;
using AdvantShop.Configuration;
using AdvantShop.Core.SQL;

namespace AdvantShop.ExportImport
{

    public class ExportFeed
    {

        public static bool IsExistsModuleSetting(string moduleName, string settingName)
        {
            var result = SQLDataAccess.ExecuteScalar<int>("SELECT COUNT([Value]) FROM [Settings].[Settings] WHERE [Name] = @settingName;",
                                                           CommandType.Text, new SqlParameter("@settingName", moduleName + settingName)) > 0;
            return result;
        }

        public static string GetModuleSetting(string moduleName, string settingName)
        {
            var result = SQLDataAccess.ExecuteScalar<string>("SELECT [Value] FROM [Settings].[Settings] WHERE [Name] = @settingName;",
                                                           CommandType.Text, new SqlParameter("@settingName", moduleName + settingName));
            return result;
        }

        public static void AddModuleSetting(string moduleName, string settingName, string settingValue)
        {
            SQLDataAccess.ExecuteNonQuery("INSERT INTO [Settings].[Settings](Name, Value) VALUES(@settingName, @settingValue);",
                                            CommandType.Text,
                                            new SqlParameter("@settingName", moduleName + settingName),
                                            new SqlParameter("@settingValue", settingValue));
        }

        public static void UpdateModuleSetting(string moduleName, string settingName, string settingValue)
        {
            SQLDataAccess.ExecuteNonQuery("UPDATE [Settings].[Settings] SET Value = @settingValue WHERE Name = @settingName;",
                                            CommandType.Text,
                                            new SqlParameter("@settingName", moduleName + settingName),
                                            new SqlParameter("@settingValue", settingValue));
        }

        public static void SetDefaultCurrencyForModule(string moduleName, string currncyValueOld)
        {
            SQLDataAccess.ExecuteNonQuery("UPDATE [Settings].[Settings] SET Value = @CurrencyNew WHERE Name = @SettingName AND Value = @CurrencyOld;",
                                            CommandType.Text,
                                            new SqlParameter("@SettingName", moduleName + "Currency"),
                                            new SqlParameter("@CurrencyOld", currncyValueOld),
                                            new SqlParameter("@CurrencyNew", SettingsCatalog.DefaultCurrencyIso3));
        }

        public static void UpdateCurrencyToDefault(string currncyValueOld)
        {
            SetDefaultCurrencyForModule("YandexMarket", currncyValueOld);
            SetDefaultCurrencyForModule("YahooShopping", currncyValueOld);
            SetDefaultCurrencyForModule("Shopzilla", currncyValueOld);
            SetDefaultCurrencyForModule("ShoppingCom", currncyValueOld);
            SetDefaultCurrencyForModule("PriceGrabber", currncyValueOld);
            SetDefaultCurrencyForModule("GoogleBase", currncyValueOld);
            SetDefaultCurrencyForModule("Amazon", currncyValueOld);
        }

        public static void RefreshModuleSetting(string moduleName, string settingName, string settingValue)
        {
            if (IsExistsModuleSetting(moduleName, settingName))
            {
                UpdateModuleSetting(moduleName, settingName, settingValue);
            }
            else
            {
                AddModuleSetting(moduleName, settingName, settingValue);
            }
        }
    }
}