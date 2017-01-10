//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Web;
using AdvantShop.Customers;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace AdvantShop.Modules
{
    public class UniSenderService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<UniSenderList> GetLists()
        {
            var lists = new List<UniSenderList>();

            try
            {
                var responseString = PostRequest("getLists");
                var response = JsonConvert.DeserializeObject<UniSenderGetListsResponse>(responseString);

                if (response.Result != null && string.IsNullOrEmpty(response.Error))
                {
                    lists = response.Result;
                    lists.Insert(0,
                        new UniSenderList
                        {
                            id = "0",
                            title = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ru" ? "Нет привязки к списку" : "No binding to the list"
                        });
                }
            }

            catch (Exception ex) { }

            return lists;
        }

        /// <summary>
        /// 2.0
        /// </summary>
        /// <param name="listid"></param>
        /// <returns></returns>
        public static List<UniSenderListMember> GetListMembers(string listid)
        {
            var listMembersLists = new List<UniSenderListMember>();
            try
            {
                var responseString = PostRequest("lists/members");

                listMembersLists = JsonConvert.DeserializeObject<List<UniSenderListMember>>(responseString);
            }
            catch (Exception ex)
            { }

            return listMembersLists;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        public static bool SubscribeListMembers(string listId, List<string> members)
        {
            return SubscribeListMembers(listId, members.Select(member => (ISubscriber)new UniSenderListMember { Email = member }).ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        public static bool SubscribeListMembers(string listId, List<ISubscriber> members)
        {
            try
            {
                foreach (var member in members)
                {
                    var responseString = PostRequest("subscribe",
                                                     new KeyValuePair<string, string>("list_ids", listId),
                                                     new KeyValuePair<string, string>("fields[email]", member.Email),
                                                     new KeyValuePair<string, string>("fields[phone]", string.IsNullOrEmpty(member.Phone) ? string.Empty : member.Phone),
                                                     new KeyValuePair<string, string>("fields[Name]", string.IsNullOrEmpty(member.FirstName) && string.IsNullOrEmpty(member.LastName) ? string.Empty : member.FirstName + " " + member.LastName),
                                                     new KeyValuePair<string, string>("double_optin", "3"));

                    var result = JsonConvert.DeserializeObject<UniSenderSubscribeResponse>(responseString);
                    if (result.Result != null && !string.IsNullOrEmpty(result.Result.Person_Id))
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        public static bool UnsubscribeListMembers(string listId)
        {
            if (listId.IsNullOrEmpty())
            {
                return false;
            }
            return UnsubscribeListMembers(listId, GetListMembers(listId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        public static bool UnsubscribeListMembers(string listId, List<UniSenderListMember> members)
        {
            try
            {
                foreach (var member in members)
                {
                    var responseString = PostRequest(
                        "exclude",
                        new KeyValuePair<string, string>("contact", member.Email),
                        new KeyValuePair<string, string>("contact_type", "email"),
                        new KeyValuePair<string, string>("list_ids", listId));

                    var result = JsonConvert.DeserializeObject<UniSenderUnsubscribeResponse>(responseString);
                    if (string.IsNullOrEmpty(result.Error) && string.IsNullOrEmpty(result.Code))
                    {
                        //allright
                    }
                }
            }
            catch (Exception ex)
            { }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        private static bool CreateCampaign(string messageId)
        {
            var responseString = PostRequest("createCampaign",
                new KeyValuePair<string, string>("message_id", messageId),
                new KeyValuePair<string, string>("track_read", "0"),
                new KeyValuePair<string, string>("track_links", "0"),
                new KeyValuePair<string, string>("defer", "1"),
                new KeyValuePair<string, string>("track_ga", "0"));

            var result = JsonConvert.DeserializeObject<UniSenderCreateCampaignResponse>(responseString);
            if (result.Result != null && string.IsNullOrEmpty(result.Error) && string.IsNullOrEmpty(result.Code))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="subject"></param>
        /// <param name="htmlContent"></param>
        /// <param name="textContent"></param>
        /// <returns></returns>
        private static string CreateEmailMessage(string listId, string subject, string htmlContent, string textContent = "")
        {
            var responseString = PostRequest("createEmailMessage",
                 new KeyValuePair<string, string>("sender_name", UniSenderSettings.FromName),
                 new KeyValuePair<string, string>("sender_email", UniSenderSettings.FromEmail),
                 new KeyValuePair<string, string>("subject", HttpUtility.UrlEncode(subject)),
                 new KeyValuePair<string, string>("body", HttpUtility.UrlEncode(htmlContent)),
                 new KeyValuePair<string, string>("list_id", listId),
                 new KeyValuePair<string, string>("text_body", textContent));


            if (!responseString.Contains("error"))
            {
                var result = JsonConvert.DeserializeObject<UniSenderCreateMessageResponse>(responseString);
                if (result != null && !string.IsNullOrEmpty(result.Result.Message_id) && string.IsNullOrEmpty(result.Error) && string.IsNullOrEmpty(result.Code))
                {
                    return result.Result.Message_id;
                }
            }

            return string.Empty;
        }

        public static bool SendMail(string listId, string subject, string htmlContent, string textContent = "")
        {
            var messageId = CreateEmailMessage(listId, subject, htmlContent, textContent);
            if (!string.IsNullOrEmpty(messageId))
            {
                return CreateCampaign(messageId);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetLang()
        {
            var lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            return new[] { "ru", "en", "it" }.Any(item => string.Equals(item, lang)) ? lang : "ru";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        private static string PostRequest(string method, params KeyValuePair<string, string>[] requestParams)
        {
            var requestString = string.Format("http://api.unisender.com/{0}/api/{1}?",
                                              GetLang(),
                                              method);

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(requestString);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                string data =
                    string.Format("format=json&api_key={0}", UniSenderSettings.ApiKey) +
                    requestParams.Aggregate(string.Empty,
                                            (current, param) =>
                                            current + string.Format("&{0}={1}", param.Key, param.Value));

                var bytes = Encoding.UTF8.GetBytes(data);

                using (var newStream = request.GetRequestStream())
                {
                    newStream.Write(bytes, 0, bytes.Length);
                }

                var responseStream = request.GetResponse().GetResponseStream();
                if (responseStream != null)
                {
                    return new StreamReader(responseStream).ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                var a = ex.Message;
                //todo: logerror
            }

            return string.Empty;
        }
    }
}