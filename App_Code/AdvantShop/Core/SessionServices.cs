﻿//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;
using AdvantShop.Configuration;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core
{
    public class SessionServices
    {
        public static double GetTimeoutSession()
        {
            var pConfig = CurrentSessionStateSection();
            if ((pConfig != null) && (pConfig.Timeout.TotalMinutes > 0))
            {
                return pConfig.Timeout.TotalMinutes;
            }

            return 5;
        }

        public static string GetSessionServiceConnectionString()
        {
            var pConfig = CurrentSessionStateSection();
            if ((pConfig.Mode == SessionStateMode.SQLServer) && (pConfig.SqlConnectionString.Length != 0))
            {
                if (pConfig.SqlConnectionString.ToLower().Contains("database") ||
                    pConfig.SqlConnectionString.ToLower().Contains("initial catalog"))
                {
                    return pConfig.SqlConnectionString;
                }
            }

            return Connection.GetConnectionString();
        }

        public static SessionStateMode GetSessionStateMode()
        {
            var pConfig = CurrentSessionStateSection();
            return pConfig.Mode;
        }

        public static void ClearOldSessionDataInDb()
        {
            using (var db = new SQLDataAccess(GetSessionServiceConnectionString()))
            {
                db.cmd.CommandText = "[dbo].[DeleteExpiredSessions]";
                db.cmd.CommandType = CommandType.StoredProcedure;
                db.cnOpen();
                db.cmd.ExecuteNonQuery();
                db.cnClose();
            }
        }

        public static SessionStateSection CurrentSessionStateSection()
        {
            return (SessionStateSection)ConfigurationManager.GetSection("system.web/sessionState");
        }

        private static DataBaseService.PingDbState TryToReachDBAtStartSession()
        {

            // Запуск первый? -------------------------------------------------------------------
            if (AppServiceStartAction.isAppFistRun)
            {
                // Да первый
                // Выставляем что первый был
                AppServiceStartAction.isAppFistRun = false;

                // Просто возвращаем результат Аппликейшена
                return AppServiceStartAction.state;
            }

            // Не первый запуск. ----------------------------------------------------------------

            // пингуем базу, что там сейчас с ней?
            var sessionStartDbRes = DataBaseService.CheckDbStates();

            if (sessionStartDbRes == DataBaseService.PingDbState.NoError)
            {
                // Все ок, база сейчас работает. а приложение было запушено?
                if (AppServiceStartAction.state != DataBaseService.PingDbState.NoError)
                {
                    // Вызываем повторный старт приложения.
                    ApplicationService.RunDbDependAppStartServices();

                    // Обновляем статус в приложении, что все хорошо, больше запускаться не нужно
                    AppServiceStartAction.state = DataBaseService.PingDbState.NoError;
                }
            }

            // Возвращаем состояние старта на сессии
            return sessionStartDbRes;

        }

        private static void SuccessStart(HttpContext current)
        {
           InitBaseSessionSettings();
        }

        public static void StartSession(HttpContext current)
        {
            if (UrlService.IsDebugUrl(current.Request.RawUrl))
                return;

            string errMsg = string.Empty;
            var ercode = TryToReachDBAtStartSession();

            // do by error
            switch (ercode)
            {
                case DataBaseService.PingDbState.NoError:
                    //if browser no suport cokkies
                    if (!current.Request.Browser.Cookies) return;
                    SuccessStart(current);
                    break;

                case DataBaseService.PingDbState.FailConnectionSqlDb:
                    current.Response.Redirect(UrlService.GetAbsoluteLink("/info/SessionError.aspx?ErrorCode=1"), true);
                    break;

                case DataBaseService.PingDbState.WrongDbVersion:
                    current.Response.Redirect(UrlService.GetAbsoluteLink("/info/SessionError.aspx?ErrorCode=2"), true);
                    break;
                    
                case DataBaseService.PingDbState.WrongDbStructure:
                    current.Response.Redirect(UrlService.GetAbsoluteLink("/info/SessionError.aspx?ErrorCode=4"), true);
                    break;

                case DataBaseService.PingDbState.Unknown:
                    current.Response.Redirect(
                        UrlService.GetAbsoluteLink(string.Format("/info/SessionError.aspx?ErrorMsg={0}",
                            HttpUtility.UrlEncode((errMsg.Length > 1000 ? errMsg.Substring(0, 1000) : errMsg) + " at SessionStart"))),
                            true);
                    break;
            }
        }

        public static void InitBaseSessionSettings()
        {
            HttpContext.Current.Session.Add("IsAdmin", false);

            //Desing

            if (CurrencyService.Currency(SettingsCatalog.DefaultCurrencyIso3) == null)
            {
                CurrencyService.RefreshCurrency();
            }

            CurrencyService.CurrentCurrency = CurrencyService.Currency(SettingsCatalog.DefaultCurrencyIso3) ?? CurrencyService.GetAllCurrencies().First();

            // Internal Settings

            HttpContext.Current.Session.Add("isDebug", false);
            HttpContext.Current.Session.Add("isAuthorize", false);
            HttpContext.Current.Session.Add("errOnAsax", false);
            HttpContext.Current.Session.Add("errMessage", "");
        }
    }
}