//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using AdvantShop.Configuration;
using Resources;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository;

namespace AdvantShop.Diagnostics
{
    /// <summary>
    /// Logger - shell for log4net logger
    /// </summary>
    public static class Debug
    {
        public enum ErrType
        {
            None,
            Err404,
            Err500,
            ErrHttp
        }

        public static readonly Dictionary<ErrType, string> ErrFile = new Dictionary<ErrType, string>
                                                      {
                                                          {ErrType.Err404 , SettingsGeneral.AbsolutePath + "/App_Data/errlog/LogErr404.csv"},
                                                          {ErrType.Err500 , SettingsGeneral.AbsolutePath + "/App_Data/errlog/LogErr500.csv"},
                                                          {ErrType.ErrHttp , SettingsGeneral.AbsolutePath + "/App_Data/errlog/LogErrHttp.csv"}
                                                      };

        private static readonly string EmailNotificationTemplate = SettingsGeneral.AbsolutePath + "/HtmlTemplate/ErrorMailTemplate.htm";

        private static readonly ILog LogErr404 = LogManager.GetLogger("LogErr404");
        private static readonly ILog LogErr500 = LogManager.GetLogger("LogErr500");
        private static readonly ILog LogErrHttp = LogManager.GetLogger("LogErrHttp");
        private static readonly ILog EmailLog = LogManager.GetLogger("EmailErr");
        public const string CharSeparate = ";";

        public static void InitLogger()
        {
            XmlConfigurator.Configure(new FileInfo(SettingsGeneral.AbsolutePath + "/log4net.config"));
        }

        public static void LogError(Exception exception, string message, bool sendEmail = true)
        {
            WriteMessage(message, exception, sendEmail);
        }

        public static void LogError(string message, bool sendEmail = true)
        {
            //WriteMessage(message, null);
            WriteMessage(message, new Exception(message), sendEmail);
        }

        public static void LogError(Exception exception, bool sendEmail = true)
        {
            WriteMessage(string.Empty, exception, sendEmail);
        }

        public static bool EnableErrorMailNotification
        {
            get { return bool.Parse(GetCoreConfigSettingValue("EnableErrorMailNotification")); }
        }

        public static int MaxMailsSendPerDay
        {
            get { return int.Parse(GetCoreConfigSettingValue("MaxMailsSendPerDay")); }
        }

        public static string GetCoreConfigSettingValue(string strKey)
        {
            var config = new AppSettingsReader();
            return (string)config.GetValue(strKey, typeof(String));
        }

        //public static bool SetCoreConfigSettingValue(string strKey, string strValue)
        //{
        //    throw (new NotImplementedException());
        //}

        private static void WriteMessage(string message, Exception exception, bool sendEmail)
        {

            var mLog = new StringBuilder();
            mLog.Append(message);
            mLog.Append(CharSeparate);
            AdvException currError = null;
            if (exception != null)
            {
                mLog.AppendFormat("\"{0}\";", exception.Message);
                currError = new AdvException(exception);
                mLog.AppendFormat("\"{0}\";", currError.ToJsonString().Replace("\"", "\"\""));
            }
            else
            {
                mLog.Append("none;none;");
            }

            var hex = exception as HttpException;
            var code = hex != null ? hex.GetHttpCode() : 0;

            switch (code)
            {
                case 0:
                    LogErrHttp.Error(mLog.ToString());
                    break;
                case 404:
                    LogErr404.Error(mLog.ToString());
                    break;
                case 500:
                    LogErr500.Error(mLog.ToString());
                    break;
                default:
                    LogErrHttp.Error(mLog.ToString());
                    break;
            }

            // Send email
            if (EnableErrorMailNotification && code != 404 && sendEmail)
            {
                if (currError != null)
                    SendMailError(currError);
            }

        }

        static readonly object Sync = new object();

        private static void SendMailError(AdvException currError)
        {

            if (HttpContext.Current != null && HttpContext.Current.Request.Url.AbsoluteUri.Contains("localhost")) return;

            lock (Sync)
            {
                var data = LogTempDataService.GetLogTempData();
                if (data.MailErrorLastSend.DayOfYear < DateTime.Now.DayOfYear) data.MailErrorCurrentCount = 0;
                if (data.MailErrorCurrentCount < MaxMailsSendPerDay)
                {

                    if (File.Exists(EmailNotificationTemplate))
                    {
                        var template = new StringBuilder(File.ReadAllText(EmailNotificationTemplate));

                        template.Replace("CommonInfo_Header", Resource.Admin_MasterPageAdmin_BugTracker_CommonInfo);
                        var outHtml = new StringBuilder();
                        outHtml.Append("<div class=\'tab-content\'>");
                        outHtml.Append("<table>");
                        outHtml.Append("<tr><td colspan='2'><hr/></td></td></tr>");
                        outHtml.AppendFormat("<tr><td><b>{0}</b></td><td>{1}</td></tr>", "Message", currError.ExceptionData.ManualMessage.IsNullOrEmpty() ? "none" : currError.ExceptionData.ManualMessage);
                        outHtml.Append("<tr><td colspan='2'><hr/></td></td></tr>");
                        outHtml.AppendFormat("<tr><td><b>{0}</b></td><td>{1}</td></tr>", "ExceptionMessage", currError.ExceptionData.ExceptionMessage);
                        outHtml.Append("<tr><td colspan='2'><hr/></td></td></tr>");
                        outHtml.AppendFormat("<tr><td><b>{0}</b></td><td>{1}</td></tr>", "ExceptionStackTrace", currError.ExceptionData.ExceptionStackTrace.IsNullOrEmpty() ? "none" : currError.ExceptionData.ExceptionStackTrace);
                        outHtml.Append("<tr><td colspan='2'><hr/></td></td></tr>");
                        outHtml.AppendFormat("<tr><td><b>{0}</b></td><td>{1}</td></tr>", "InnerExceptionMessage", currError.ExceptionData.InnerExceptionMessage.IsNullOrEmpty() ? "none" : currError.ExceptionData.InnerExceptionMessage);
                        outHtml.Append("<tr><td colspan='2'><hr/></td></td></tr>");
                        outHtml.AppendFormat("<tr><td><b>{0}</b></td><td>{1}</td></tr>", "InnerExceptionStackTrace", currError.ExceptionData.InnerExceptionStackTrace.IsNullOrEmpty() ? "none" : currError.ExceptionData.InnerExceptionStackTrace);
                        outHtml.Append("<tr><td colspan='2'><hr/></td></td></tr>");
                        foreach (var key in currError.ExceptionData.Parameters.Keys)
                            outHtml.AppendFormat("<tr><td><b>{0}</b></td><td>{1}</td></tr>", key, currError.ExceptionData.Parameters[key]);
                        outHtml.Append("<tr><td colspan='2'><hr/></td></td></tr>");

                        outHtml.Append("</table>");
                        outHtml.Append("</div>");
                        template.Replace("CommonInfo_Body", outHtml.ToString());


                        template.Replace("Request_Header", Resource.Admin_MasterPageAdmin_BugTracker_Request);
                        outHtml = new StringBuilder();
                        outHtml.Append("<div class=\'tab-content\'>");
                        if (currError.RequestData != null)
                        {
                            outHtml.Append("<table>");

                            foreach (var key in currError.RequestData.ColectionData.Keys)
                                outHtml.AppendFormat("<tr><td><b>{0}</b></td><td>{1}</td></tr>", key, currError.RequestData.ColectionData[key]);
                            outHtml.Append("</table>");
                        }
                        outHtml.Append("</div>");
                        template.Replace("Request_Body", outHtml.ToString());


                        template.Replace("Browser_Header", Resource.Admin_MasterPageAdmin_BugTracker_Browser);
                        outHtml = new StringBuilder();
                        outHtml.Append("<div class=\'tab-content\'>");
                        if (currError.BrowserData != null)
                        {
                            outHtml.Append("<table>");
                            foreach (var key in currError.BrowserData.ColectionData.Keys)
                                outHtml.AppendFormat("<tr><td><b>{0}</b></td><td>{1}</td></tr>", key, currError.BrowserData.ColectionData[key]);
                            outHtml.Append("</table>");
                        }
                        outHtml.Append("</div>");
                        template.Replace("Browser_Body", outHtml.ToString());


                        template.Replace("Session_Header", Resource.Admin_MasterPageAdmin_BugTracker_Session);
                        outHtml = new StringBuilder();
                        outHtml.Append("<div class=\'tab-content\'>");
                        if (currError.SessionData != null)
                        {
                            outHtml.Append("<table>");
                            foreach (var key in currError.SessionData.ColectionData.Keys)
                                outHtml.AppendFormat("<tr><td><b>{0}</b></td><td>{1}</td></tr>", key, currError.SessionData.ColectionData[key]);
                            outHtml.Append("</table>");
                        }
                        outHtml.Append("</div>");
                        template.Replace("Session_Body", outHtml.ToString());

                        ILoggerRepository rep = LogManager.GetRepository();
                        foreach (IAppender appender in rep.GetAppenders())
                        {
                            if (appender is SmtpAppender)
                            {
                                // Loop exception here. SettingsMain.SiteUrl - Лезет в базу.
                                // ((SmtpAppender)appender).Subject = SettingsMain.SiteUrl + " " + SettingsGeneral.SiteVersion;

                                ((SmtpAppender)appender).Subject = SettingsGeneral.AbsoluteUrl + " " + SettingsGeneral.SiteVersion;
                                var emailLog1 = LogManager.GetLogger("EmailErr");
                                emailLog1.Error(template.ToString());
                            }
                        }
                    }

                    data.MailErrorCurrentCount++;
                    data.MailErrorLastSend = DateTime.Now;
                }
                LogTempDataService.UpdateLogTempData(data);
            }
        }
    }
}
