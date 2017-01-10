﻿//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Modules
{
    public class ModuleSettingsProvider
    {
        public static T GetSettingValue<T>(string strKey, string moduleName)
        {
            object obj = GetSqlSettingValue(strKey, moduleName);
            if (obj == null)
            {
                return default(T);
            }

            return (T) Convert.ChangeType(obj, typeof (T));
        }

        public static void SetSettingValue<T>(string strKey, T strValue, string moduleName)
        {
            SetSqlSettingValue(strKey, strValue, moduleName);
            CacheManager.Remove(CacheNames.GetModuleSettingsCacheObjectName());
        }

        /// <summary>
        /// Save settings into DB
        /// </summary>
        private static void SetSqlSettingValue<T>(string strName, T strValue, string moduleName)
        {
            SQLDataAccess.ExecuteNonQuery("[Settings].[sp_UpdateModuleSettings]", CommandType.StoredProcedure,
                new SqlParameter("@Name", strName),
                new SqlParameter("@Value", strValue.ToString()),
                new SqlParameter("@ModuleName", moduleName));
        }

        /// <summary>
        /// Read settings from DB.
        /// On Err: Function will return Nothing
        /// </summary>
        private static object GetSqlSettingValue(string strName, string moduleName)
        {
            var strCacheName = CacheNames.GetModuleSettingsCacheObjectName();

            var settings = CacheManager.Contains(strCacheName)
                                ? CacheManager.Get<List<ModuleSettings>>(strCacheName)
                                : GeSettingValuesList();

            var setting = settings.FirstOrDefault(s => s.Name == strName && s.ModuleName == moduleName);
            if (setting != null)
                return setting.Value;
            
            return null;
        }

        private static List<ModuleSettings> GeSettingValuesList()
        {
            var settings = SQLDataAccess.ExecuteReadList("Select * From [Settings].[ModuleSettings]", CommandType.Text,
                reader => new ModuleSettings()
                {
                    Name = SQLDataHelper.GetString(reader, "Name"),
                    Value = SQLDataHelper.GetObject(reader, "Value"),
                    ModuleName = SQLDataHelper.GetString(reader, "ModuleName"),
                });
            if (settings != null)
                CacheManager.Insert(CacheNames.GetModuleSettingsCacheObjectName(), settings);
            return settings;
        }

        private static object GetSqlSettingValueFromDb(string strName, string moduleName)
        {
            return SQLDataAccess.ExecuteScalar("[Settings].[sp_GetModuleSettingValue]", CommandType.StoredProcedure,
                new SqlParameter("@SettingName", strName),
                new SqlParameter("@ModuleName", moduleName));
        }

        public static bool IsSqlSettingExist(string strKey, string moduleName)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(Name) AS COUNTID FROM [Settings].[ModuleSettings] WHERE [Name] = @Name AND [ModuleName] = @ModuleName",
                CommandType.Text,
                new SqlParameter("@Name", strKey),
                new SqlParameter("@ModuleName", moduleName)) > 0;
        }

        public static bool RemoveSqlSetting(string strKey, string moduleName)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Settings].[ModuleSettings] WHERE [Name] = @Name AND [ModuleName] = @ModuleName",
                CommandType.Text,
                new SqlParameter("@Name", strKey),
                new SqlParameter("@ModuleName", moduleName));

            CacheManager.Remove(CacheNames.GetModuleSettingsCacheObjectName());
            return true;
        }

        public static string GetAbsolutePath()
        {
            return SettingsGeneral.AbsolutePath;
        }
    }
}