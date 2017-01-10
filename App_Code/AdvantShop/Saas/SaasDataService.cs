//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;

namespace AdvantShop.SaasData
{
    public class SaasDataService
    {
        private static readonly object SyncObject = new object();

        private const string RequestUrl = "http://modules.advantshop.net/Saas/GetParams/";
        public static bool IsSaasEnabled
        {
            get { return ModeConfigService.IsModeEnabled(ModeConfigService.Modes.SaasMode); }
        }

        public static bool IsExist()
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT COUNT(*) FROM [dbo].[SaasData]", CommandType.Text) > 0;
        }

        public static Dictionary<string, string> GetDbSaasdata()
        {
            if (IsExist())
                return SQLDataAccess.ExecuteReadDictionary<string, string>("Select [Key],[Value] from dbo.SaasData", CommandType.Text, "Key", "Value");
            else
                return null;
        }

        private static void AddSingle(string key, string value)
        {
            SQLDataAccess.ExecuteNonQuery("Insert into dbo.SaasData ([key],value) Values (@key,@value) ", CommandType.Text,
                                            new SqlParameter("@key", key), new SqlParameter("@value", value));
        }

        private static void UpdateSingle(string key, string value)
        {
            SQLDataAccess.ExecuteNonQuery("Update dbo.SaasData set value=@value where [key]=@key", CommandType.Text,
                                            new SqlParameter("@key", key), new SqlParameter("@value", value));
        }

        private static bool IsExistSingle(string key)
        {
            return SQLDataAccess.ExecuteScalar<int>("select count([key]) from dbo.SaasData where [key]=@key", CommandType.Text, new SqlParameter("@key", key)) > 0;
        }

        public static void SetData(Dictionary<string, string> saasData)
        {
            if (saasData.ContainsKey(SaasDataTemplate.Error))
                return;
            if (saasData.Keys.Count > 1)
                ClearSaasData();

            foreach (var key in saasData.Keys)
            {
                var value = saasData[key];
                if (IsExistSingle(key)) UpdateSingle(key, value);
                else AddSingle(key, value);
            }
        }

        public static void ClearSaasData()
        {
            SQLDataAccess.ExecuteNonQuery("Delete from dbo.SaasData", CommandType.Text);
        }

        public static SaasData CurrentSaasData
        {
            get
            {
                lock (SyncObject)
                {
                    if (HttpContext.Current != null)
                    {
                        var contextSaasData = HttpContext.Current.Items["SaasData"] as SaasData;
                        if (contextSaasData != null) return contextSaasData;
                        var dbSaasData = GetSaasData();
                        HttpContext.Current.Items["SaasData"] = dbSaasData;
                        return dbSaasData;
                    }
                    return GetSaasData();
                }
            }
        }

        public static SaasData GetSaasData(bool forceUpdate = false)
        {
            var saasDataDb = GetDbSaasdata();
            var saasData = new SaasData(saasDataDb);

            SaasData saasDataNew = null;
            DateTime now = DateTime.Now;
            if ((saasData.LastUpdate <= now) || !saasData.IsWorkingNow || forceUpdate)
            {
                saasDataNew = UpdateSaasDataFromService();
                if (saasDataNew != null)
                {
                    if (IsExistSingle(SaasDataTemplate.LastUpdate))
                    {
                        UpdateSingle(SaasDataTemplate.LastUpdate, now.AddMinutes(5).ToString(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        AddSingle(SaasDataTemplate.LastUpdate, now.AddMinutes(5).ToString(CultureInfo.InvariantCulture));
                    }
                }
            }
            return saasDataNew ?? saasData;
        }

        private static SaasData UpdateSaasDataFromService()
        {
            try
            {
                var key = SettingsLic.LicKey;
                var request = WebRequest.Create(RequestUrl + key);
                request.Method = "GET";

                using (var dataStream = request.GetResponse().GetResponseStream())
                {
                    using (var reader = new StreamReader(dataStream))
                    {
                        var responseFromServer = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(responseFromServer))
                        {
                            var newSaasData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseFromServer);
                            SetData(newSaasData);
                            return new SaasData(newSaasData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            return null;
        }
    }
}
