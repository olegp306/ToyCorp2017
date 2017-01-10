//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.Mails
{
    public class SendMail
    {
        public static bool SendMailThread(string strTo, string strSubject, string strText, bool isBodyHtml, string smtpServer, string login, string password, int port, string emailFrom, bool ssl)
        {
            return (SendMailThreadStringResult(strTo, strSubject, strText, isBodyHtml, smtpServer, login, password, port, emailFrom, ssl) == "True");
        }

        public static string SendMailThreadStringResult(string strTo, string strSubject, string strText, bool isBodyHtml, string smtpServer, string login, string password, int port, string emailFrom, bool ssl)
        {
            string strResult = "True";

            try
            {
                using (var emailClient = new SmtpClient(smtpServer)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(login, password),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Port = port,
                    EnableSsl = ssl
                })
                {
                    string[] strMails = strTo.Split(';');
                    foreach (string strEmail in strMails)
                    {
                        string strE = strEmail.Trim();
                        if (string.IsNullOrEmpty(strE)) continue;

                        if (!ValidationHelper.IsValidEmail(strE)) continue;
                        using (var message = new MailMessage(new MailAddress(emailFrom, emailFrom), new MailAddress(strE)))
                        {
                            message.Subject = strSubject;
                            message.Body = strText;
                            message.IsBodyHtml = isBodyHtml;
                            emailClient.Send(message);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                strResult = ex.Message;
                Debug.LogError(ex, false);
            }

            return strResult;
        }

        public static bool SendMailInTask(string strTo, string strSubject, string strText, bool isBodyHtml, string setSmtpServer, int setPort, string setLogin, string setPassword, string setEmailFrom, bool setSsl)
        {
            Task.Factory.StartNew(() => SendMailThread(strTo, strSubject, strText, isBodyHtml, setSmtpServer, setLogin, setPassword, setPort, setEmailFrom, setSsl));
            return true;
        }

        public static bool SendMailNow(string strTo, string strSubject, string strText, bool isBodyHtml)
        {
            string smtp = SettingsMail.SMTP;
            string login = SettingsMail.Login;
            string password = SettingsMail.Password;
            int port = SettingsMail.Port;
            string email = SettingsMail.From;
            bool ssl = SettingsMail.SSL;
            return SendMailInTask(strTo, strSubject, strText, isBodyHtml, smtp, port, login, password, email, ssl);
        }
    }
}