//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using Newtonsoft.Json;

namespace AdvantShop.Security.OpenAuth
{
    public class GoogleOauth
    {
        private class GoogleOauthResponseAccessToken
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public string expires_in { get; set; }
            public string id_token { get; set; }
        }

        private class GoogleOauthResponseIdToken
        {
            public string email_verified { get; set; }
            public string email { get; set; }
            public string issued_at { get; set; }
            public string expires_in { get; set; }
            public string user_id { get; set; }
            public string audience { get; set; }
            public string issued_to { get; set; }
        }

        private class GoogleOauthResponseUserProfile
        {
            public string sub { get; set; }
            public string name { get; set; }
            public string given_name { get; set; }
            public string family_name { get; set; }
            public string profile { get; set; }
            public string picture { get; set; }
            public string email { get; set; }
            public bool email_verified { get; set; }
            public string gender { get; set; }
            public string locale { get; set; }
        }


        public static void SendAuthenticationRequest()
        {
            HttpContext.Current.Response.Redirect(
                string.Format(@"https://accounts.google.com/o/oauth2/auth?client_id={0}&response_type={1}&scope={2}&redirect_uri={3}&state={4}&display={5}",
                    SettingsOAuth.GoogleClientId,
                    "code",
                    "openid%20email%20profile",
                    StringHelper.MakeASCIIUrl(SettingsMain.SiteUrl) + "/Login.aspx?auth=google",
                    SettingsMain.SiteUrl.GetHashCode(),
                    "popup"));
        }

        public static void ExchangeCodeForAccessToken(string code)
        {
            var data = string.Format(
                "code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type={4}",
                code,
                SettingsOAuth.GoogleClientId,
                SettingsOAuth.GoogleClientSecret,
                StringHelper.MakeASCIIUrl(SettingsMain.SiteUrl) + "/Login.aspx?auth=google",
                "authorization_code");

            var request = WebRequest.Create("https://accounts.google.com/o/oauth2/token?");

            request.Method = "POST";

            byte[] byteArray = Encoding.UTF8.GetBytes(data);

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }
            
            try
            {
                var response = request.GetResponse();

                var responseResult = JsonConvert.DeserializeObject<GoogleOauthResponseAccessToken>(
                    new StreamReader(response.GetResponseStream()).ReadToEnd());

                var userProfile = GetUserInformation(responseResult.access_token);
                OAuthResponce.AuthOrRegCustomer(new Customer
                {
                    FirstName = userProfile.name,
                    LastName = userProfile.family_name,
                    EMail = userProfile.email,
                    CustomerGroupId = 1,
                    Password = Guid.NewGuid().ToString()
                });

            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ad_token"></param>
        /// <returns></returns>
        private static GoogleOauthResponseIdToken GetTokenIdInfo(string ad_token)
        {
            var request = WebRequest.Create("https://www.googleapis.com/oauth2/v1/tokeninfo?id_token=" + ad_token);

            request.Method = "GET";

            var response = request.GetResponse();

            return JsonConvert.DeserializeObject<GoogleOauthResponseIdToken>(
                    new StreamReader(response.GetResponseStream()).ReadToEnd());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private static GoogleOauthResponseUserProfile GetUserInformation(string accessToken)
        {
            var request = WebRequest.Create("https://www.googleapis.com/oauth2/v3/userinfo?access_token=" + accessToken);

            request.Method = "GET";

            var response = request.GetResponse();

            return JsonConvert.DeserializeObject<GoogleOauthResponseUserProfile>(
                    new StreamReader(response.GetResponseStream()).ReadToEnd());
        }
    }
}