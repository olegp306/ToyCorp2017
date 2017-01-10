//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using CBR;

namespace AdvantShop.Repository.Currencies
{
    public class CurrencyService
    {
        public static string DefaultPriceFormat
        {
            get { return @"##,##0.00 "; }
        }

        private static string CurrencyCookieName
        {
            get { return HttpUtility.UrlEncode(SettingsMain.SiteUrl) + "_Currency"; }
        }

        public static void RefreshCurrency()
        {
            var currencies = SQLDataAccess.ExecuteReadList("SELECT * FROM Catalog.Currency",
                                                        CommandType.Text, GetCurrencyFromReader);
            foreach (Currency t in currencies)
                HttpContext.Current.Application["Currency_" + t.Iso3] = t;
        }

        private static Currency GetCurrencyFromReader(IDataReader reader)
        {
            return new Currency()
            {
                CurrencyID = SQLDataHelper.GetInt(reader, "CurrencyID"),
                Value = SQLDataHelper.GetFloat(reader, "CurrencyValue"),
                Iso3 = SQLDataHelper.GetString(reader, "CurrencyIso3"),
                Symbol = SQLDataHelper.GetString(reader, "Code"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                PriceFormat = SQLDataHelper.GetString(reader, "PriceFormat", string.Empty),
                IsCodeBefore = SQLDataHelper.GetBoolean(reader, "IsCodeBefore"),
                NumIso3 = SQLDataHelper.GetInt(reader, "CurrencyNumIso3")
            };
        }

        public static Currency CurrentCurrency
        {
            get
            {
                var cookieCurrency = CommonHelper.GetCookie(CurrencyCookieName);
                var code = (cookieCurrency != null && !string.IsNullOrEmpty(cookieCurrency.Value)) ? cookieCurrency.Value : "MissingCurrency";

                if (HttpContext.Current == null)
                {
                    var currency = GetAllCurrencies(true).FirstOrDefault(x => x.Iso3 == SettingsCatalog.DefaultCurrencyIso3);
                    if (currency == null)
                        return GetAllCurrencies(true).FirstOrDefault();

                    return currency;
                }

                return (Currency)HttpContext.Current.Application["Currency_" + code] ?? GetAllCurrencies().FirstOrDefault();
            }
            set
            {
                CommonHelper.SetCookie(CurrencyCookieName, value.Iso3);
            }
        }

        public static Currency BaseCurrency
        {
            get
            {
                var currency = GetAllCurrencies(true).FirstOrDefault(x => x.Value == 1);
                if (currency == null)
                    return GetAllCurrencies(true).FirstOrDefault();

                return currency;
            }
        }

        public static Currency Currency(string iso3)
        {
            return (Currency)HttpContext.Current.Application["Currency_" + iso3];
        }

        public static List<Currency> GetAllCurrencies(bool fromCache)
        {
            if (!fromCache) return GetAllCurrencies();

            string strCacheName = CacheNames.GetCurrenciesCacheObjectName();
            List<Currency> res;
            if (CacheManager.Contains(strCacheName))
            {
                res = CacheManager.Get<List<Currency>>(strCacheName);
                if (res != null)
                    return res;
            }
            res = GetAllCurrencies();

            if (res != null)
                CacheManager.Insert(strCacheName, res);
            else
                CacheManager.Remove(strCacheName);

            return res;
        }

        public static List<Currency> GetAllCurrencies()
        {
            return SQLDataAccess.ExecuteReadList<Currency>("SELECT Catalog.Currency.* FROM Catalog.Currency", CommandType.Text, GetCurrencyFromReader);
        }

        public static Currency GetCurrency(int idCurrency)
        {
            return SQLDataAccess.ExecuteReadOne<Currency>("SELECT Catalog.Currency.* FROM Catalog.Currency where  CurrencyID = @id", CommandType.Text, GetCurrencyFromReader, new SqlParameter("@id", idCurrency));
        }

        public static Currency GetCurrencyByIso3(string iso3)
        {
            return SQLDataAccess.ExecuteReadOne<Currency>("SELECT Catalog.Currency.* FROM Catalog.Currency where  CurrencyIso3 = @iso3", CommandType.Text, GetCurrencyFromReader, new SqlParameter("@iso3", iso3));
        }

        public static IEnumerable<int> GetAllCurrencyId()
        {

            return SQLDataAccess.ExecuteReadColumnIEnumerable<int>("SELECT CurrencyID FROM [Catalog].[Currency]", CommandType.Text, "CurrencyID");
        }

        public static void DeleteCurrency(int idCurrency)
        {
            var currentCurrency = GetCurrency(idCurrency);
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Catalog].[Currency] where CurrencyID = @id", CommandType.Text, new SqlParameter("@id", idCurrency));
            if (currentCurrency != null)
                ExportImport.ExportFeed.UpdateCurrencyToDefault(currentCurrency.Iso3);
        }

        public static void UpdateCurrency(Currency currency)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Catalog].[Currency] set Name=@name, Code=@code, CurrencyValue=@value, CurrencyIso3=@ISO3, CurrencyNumIso3=@CurrencyNumIso3, IsCodeBefore=@isCodeBefore, PriceFormat=@format where CurrencyID = @id",
                CommandType.Text,
                new SqlParameter("@id", currency.CurrencyID),
                new SqlParameter("@name", currency.Name),
                new SqlParameter("@code", currency.Symbol),
                new SqlParameter("@value", currency.Value),
                new SqlParameter("@ISO3", currency.Iso3),
                new SqlParameter("@CurrencyNumIso3", currency.NumIso3 != 0 ? currency.NumIso3 : new Random().Next(1, 999)),
                new SqlParameter("@isCodeBefore", currency.IsCodeBefore),
                new SqlParameter("@format", currency.PriceFormat));
        }

        public static void InsertCurrency(Currency currency)
        {
            currency.CurrencyID = SQLDataAccess.ExecuteScalar<int>(
                 "INSERT INTO [Catalog].[Currency] (Name, Code, CurrencyValue, CurrencyIso3, CurrencyNumIso3, IsCodeBefore, PriceFormat) VALUES (@Name, @Code, @CurrencyValue, @CurrencyIso3, @CurrencyNumIso3, @IsCodeBefore, @PriceFormat);SELECT scope_identity();",
                 CommandType.Text,
                 new SqlParameter("@Name", currency.Name),
                 new SqlParameter("@Code", currency.Symbol),
                 new SqlParameter("@CurrencyValue", currency.Value),
                 new SqlParameter("@CurrencyIso3", currency.Iso3),
                 new SqlParameter("@CurrencyNumIso3", currency.NumIso3 != 0 ? (object)currency.NumIso3 : DBNull.Value),
                 new SqlParameter("@isCodeBefore", currency.IsCodeBefore),
                 new SqlParameter("@PriceFormat", currency.PriceFormat));
        }

        public static void UpdateCurrenciesFromCentralBank()
        {
            var di = new DailyInfo();
            DataSet ds = di.GetCursOnDate(DateTime.Now);

            try
            {
                var rub = GetCurrencyByIso3("RUB");
                var rubScale = rub != null ? rub.Value : 1;
                using (var db = new SQLDataAccess())
                {
                    db.cmd.CommandText = "SELECT CurrencyIso3,[CurrencyValue] FROM Catalog.Currency";
                    db.cmd.CommandType = CommandType.Text;
                    db.cmd.Parameters.Clear();

                    using (var dbUpd = new SQLDataAccess())
                    {
                        dbUpd.cmd.CommandText = "UPDATE Catalog.Currency SET CurrencyValue=@CurrencyValue, CurrencyNumIso3=@CurrencyNumIso3 WHERE CurrencyIso3=@CurrencyIso3";
                        dbUpd.cmd.CommandType = CommandType.Text;

                        db.cnOpen();
                        using (var read = db.cmd.ExecuteReader())
                        {
                            while (read.Read())
                            {
                                foreach (DataRow row in ds.Tables["ValuteCursOnDate"].Rows)
                                {
                                    if (read["CurrencyIso3"].ToString().ToLower() != row["VchCode"].ToString().ToLower()) continue;
                                    dbUpd.cmd.Parameters.Clear();
                                    dbUpd.cmd.Parameters.AddWithValue("@CurrencyValue", SQLDataHelper.GetFloat(row["Vcurs"]) / SQLDataHelper.GetFloat(row["Vnom"]) * rubScale);
                                    dbUpd.cmd.Parameters.AddWithValue("@CurrencyIso3", row["VchCode"].ToString());
                                    dbUpd.cmd.Parameters.AddWithValue("@CurrencyNumIso3", row["VCode"].ToString());
                                    dbUpd.cnOpen();
                                    dbUpd.cmd.ExecuteNonQuery();
                                    dbUpd.cnClose();
                                }
                            }
                            read.Close();
                        }
                    }
                    db.cnClose();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        public static float ConvertCurrency(float sum, string newCurrencyCode, string oldCurrencyCode)
        {
            return sum * Currency(oldCurrencyCode).Value / Currency(newCurrencyCode).Value;
        }
        public static float ConvertCurrency(float sum, float newCurrencyValue, float oldCurrencyValue)
        {
            return sum * oldCurrencyValue / newCurrencyValue;
        }
    }
}

