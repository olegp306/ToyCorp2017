//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using Newtonsoft.Json;

namespace AdvantShop.Security.OpenAuth
{
    public class VkontakteOauth
    {
        public class VkontakteUser
        {
            public string uid { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
        }

        public class Vk_responce
        {
            public string access_token { get; set; }
            public string expires_in { get; set; }
            public string user_id { get; set; }
        }

        public class VkontakteUserResponse
        {
            public List<VkontakteUser> response;
        }

        public static void VkontakteAuth(string code, string email, string redirectUrl)
        {
            try
            {
                var request = WebRequest.Create(string.Format("https://oauth.vk.com/access_token?client_id={0}&client_secret={1}&code={2}&redirect_uri={3}",
                                                                SettingsOAuth.VkontakeClientId,
                                                                SettingsOAuth.VkontakeSecret,
                                                                code,
                                                                StringHelper.MakeASCIIUrl(SettingsMain.SiteUrl) + "/" + redirectUrl + "?auth=vk"));
                request.Method = "GET";

                var response = request.GetResponse();
                if (response != null)
                {
                    var str = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    var vkr = JsonConvert.DeserializeObject<Vk_responce>(str);

                    VkontakteAuthGetProfiles(vkr.access_token, vkr.user_id, email);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex + "   code:" + code + "   redirectUrl:" + StringHelper.MakeASCIIUrl(SettingsMain.SiteUrl) + "/" + redirectUrl + "?auth=vk");
            }
        }

        private static void VkontakteAuthGetProfiles(string access_token, string user_id, string email)
        {
            try
            {
                var request = WebRequest.Create(string.Format("https://api.vk.com/method/getProfiles?uid={0}&access_token={1}",
                                                                user_id,
                                                                access_token));

                request.Method = "GET";

                var response = request.GetResponse();
                if (response != null)
                {
                    var vkontakteUser = JsonConvert.DeserializeObject<VkontakteUserResponse>(
                   (new StreamReader(response.GetResponseStream()).ReadToEnd()));
                    OAuthResponce.AuthOrRegCustomer(new Customer
                    {
                        FirstName = vkontakteUser.response[0].first_name,
                        LastName = vkontakteUser.response[0].last_name,
                        EMail = vkontakteUser.response[0].uid + "@temp.vkontakte",
                        CustomerGroupId = 1,
                        Password = Guid.NewGuid().ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        public static void VkontakteAuthDialog(string redirectUrl)
        {
            HttpContext.Current.Response.Redirect(
                string.Format(
                    @"https://oauth.vk.com/authorize?client_id={0}&scope={1}&redirect_uri={2}&response_type=code",
                    SettingsOAuth.VkontakeClientId,
                    "offline",
                     StringHelper.MakeASCIIUrl(SettingsMain.SiteUrl) + "/" + redirectUrl + "?auth=vk"));
        }
    }
}