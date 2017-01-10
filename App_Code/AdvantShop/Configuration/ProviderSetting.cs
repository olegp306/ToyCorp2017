//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Configuration;
using System.Xml.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using Quartz.Util;

namespace AdvantShop.Configuration
{
    /// <summary>
    /// Setting provider
    /// </summary>
    /// <remarks></remarks>
    public class SettingProvider
    {
        private const string ConfigSettingValueCacheKey = "ConfigSettingValue_";
        private const string SettingsCacheKey = "AllSettings_";

        public sealed class SettingIndexer
        {
            public string this[string name]
            {
                get { return GetSqlSettingValue(name); }
                set { SetSqlSettingValue(name, value); }
            }
        }

        private static SettingIndexer _staticIndexer;
        public static SettingIndexer Items
        {
            get { return _staticIndexer ?? (_staticIndexer = new SettingIndexer()); }
        }

        #region  SQL storage

        /// <summary>
        /// Save settings into DB
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <remarks></remarks>
        public static void SetSqlSettingValue(string name, string value)
        {
            SQLDataAccess.ExecuteNonQuery("[Settings].[sp_UpdateSettings]", CommandType.StoredProcedure,
                                            new SqlParameter("@Name", name.Trim()), new SqlParameter("@Value", value));

            CacheManager.RemoveByPattern(SettingsCacheKey);
        }

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <param name="strName"></param>
        /// <returns></returns>
        public static string GetSqlSettingValue(string strName)
        {
            var key = strName.Trim();

            var settings = GetAllSettings();

            if (settings != null && settings.ContainsKey(key))
                return settings[key];

            return null;
        }

        private static Dictionary<string, string> GetAllSettings()
        {
            if (CacheManager.Contains(SettingsCacheKey))
                return CacheManager.Get<Dictionary<string, string>>(SettingsCacheKey);


            var settings =
                SQLDataAccess.ExecuteReadDictionary<string, string>("SELECT [Name],[Value] FROM [Settings].[Settings]",
                    CommandType.Text, "Name", "Value");

            if (settings == null)
                return new Dictionary<string, string>();

            CacheManager.Insert(SettingsCacheKey, settings);
            return settings;
        }

        /*
        public static bool IsSqlSettingExist(string strKey)
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT COUNT(Name) AS COUNTID FROM [Settings].[Settings] WHERE [Name]=@Name",
                                                   CommandType.Text, new SqlParameter("@Name", strKey)) > 0;
        }

        public static bool RemoveSqlSetting(string strKey)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Settings].[Settings] WHERE [Name]=@Name", CommandType.Text, new SqlParameter("@Name", strKey));

            CacheManager.Remove(CacheNames.GetCommonSettingsCacheObjectName(strKey));
            return true;
        }
        */
        #endregion

        #region  Web.config storage

        /// <summary>
        /// Read settings from appSettings node web.config file.
        /// On Err: Function will return an empty string
        /// </summary>
        /// <param name="strKey"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetConfigSettingValue(string strKey)
        {
            var cacheKey = ConfigSettingValueCacheKey + strKey;
            if (CacheManager.Contains(cacheKey))
                return CacheManager.Get<string>(cacheKey);

            var config = new AppSettingsReader();
            var value = config.GetValue(strKey, typeof(String)).ToString();
            CacheManager.Insert(cacheKey, value);

            return value;
        }
        
        public static T GetConfigSettingValue<T>(string strKey)
        {
            var cacheKey = ConfigSettingValueCacheKey + strKey;
            if (CacheManager.Contains(cacheKey))
                return CacheManager.Get<T>(cacheKey);

            var config = new AppSettingsReader();
            var value = (T)config.GetValue(strKey, typeof(T));
            CacheManager.Insert(cacheKey, value);

            return value;
        }

        /// <summary>
        /// Save settings from appSettings node web.config
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="strValue"></param>
        /// <remarks></remarks>
        public static bool SetConfigSettingValue(string strKey, string strValue)
        {
            System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
            var myAppSettings = (AppSettingsSection)config.GetSection("appSettings");
            myAppSettings.Settings[strKey].Value = strValue;
            config.Save();

            return true;
        }

        #endregion
        
        public static CustomerContact GetSellerContact()
        {
            return new CustomerContact()
            {
                CountryId = SettingsMain.SellerCountryId,
                RegionId = SettingsMain.SellerRegionId
            };
        }
    }
}