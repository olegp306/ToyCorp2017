//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.IO;
using System.Net;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using System.Web;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace AdvantShop.Security.OpenAuth
{
    public class OdnoklassnikiResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
    }

    public class OdnoklassnikiUser
    {
        public string uid { get; set; }
        public string birthday { get; set; }
        public string age { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string name { get; set; } //composition of first and last name to render
        public bool has_email { get; set; }//true/false has or not e-mail
        public string gender { get; set; }
        public string pic_1 { get; set; } //profile small icon (50x50)
        public string pic_2 { get; set; } //profile small picture (128x128)
    }

    public class OdnoklassnikiOauth
    {
        public static void OdnoklassnikiLogin(string code)
        {
            var request =
                WebRequest.Create(string.Format("http://api.odnoklassniki.ru/oauth/token.do?code={0}&redirect_uri={1}&grant_type={2}&client_id={3}&client_secret={4}",
                code,
                HttpUtility.UrlEncode(StringHelper.MakeASCIIUrl(SettingsMain.SiteUrl) + "/Login.aspx?auth=od"),
                "authorization_code",
                SettingsOAuth.OdnoklassnikiClientId,
                SettingsOAuth.OdnoklassnikiSecret));

            request.Method = "POST";

            var response = request.GetResponse();

            using (var responseStream = new StreamReader(response.GetResponseStream()))
            {
                var odnoklassnikiResponse = JsonConvert.DeserializeObject<OdnoklassnikiResponse>(responseStream.ReadToEnd());
                if (!string.IsNullOrWhiteSpace(odnoklassnikiResponse.access_token))
                {
                    OdnoklassnikiGetAndLoginUser(odnoklassnikiResponse.access_token);
                }
            }
        }

        private static void OdnoklassnikiGetAndLoginUser(string access_token)
        {
            var sigString = string.Format("application_key={0}method={1}{2}",
                SettingsOAuth.OdnoklassnikiPublicApiKey,
                "users.getCurrentUser",
                GetMd5Hash(MD5.Create(), access_token + SettingsOAuth.OdnoklassnikiSecret));

            var request =
                WebRequest.Create(string.Format("http://api.odnoklassniki.ru/fb.do?method={0}&access_token={1}&application_key={2}&sig={3}",
                "users.getCurrentUser",
                access_token,
                SettingsOAuth.OdnoklassnikiPublicApiKey,
                GetMd5Hash(MD5.Create(), sigString).ToLower()));

            request.Method = "GET";

            var response = request.GetResponse();
            
            using (var responseStream = new StreamReader(response.GetResponseStream()))
            {
                var odnoklassnikiResponse = JsonConvert.DeserializeObject<OdnoklassnikiUser>(responseStream.ReadToEnd());
                OAuthResponce.AuthOrRegCustomer(new Customer
                {
                    FirstName = odnoklassnikiResponse.first_name,
                    LastName = odnoklassnikiResponse.last_name,
                    EMail = odnoklassnikiResponse.uid + "@temp.odnoklassniki",
                    CustomerGroupId = 1,
                    Password = Guid.NewGuid().ToString()
                });
            }
        }

        public static void OdnoklassnikiAuthDialog()
        {
            HttpContext.Current.Response.Redirect(
                string.Format("http://www.odnoklassniki.ru/oauth/authorize?client_id={0}&response_type={1}&redirect_uri={2}",
                SettingsOAuth.OdnoklassnikiClientId,
                "code",
                StringHelper.MakeASCIIUrl(SettingsMain.SiteUrl) + "/Login.aspx?auth=od"));
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}