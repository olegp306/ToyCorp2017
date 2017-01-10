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
using AdvantShop.Diagnostics;

namespace AdvantShop.Security.OpenAuth
{
    public class OAuthRequest
    {
        private readonly Dictionary<Providers, string> _providerEndPoint = new Dictionary<Providers, string>
                                                                    {
                                                                        {Providers.Google, "https://www.google.com/accounts/o8/ud"},
                                                                        {Providers.Yandex, "http://openid.yandex.ru/server/"},
                                                                        {Providers.Rambler, "http://rambler.ru"},
                                                                       {Providers.Mail, "http://openid.mail.ru/login"}
                                                                       
                                                                    };

        public enum Providers
        {
            Empty,
            Google,
            Yandex,
            Rambler,
            Mail
        }

        public string UserId { get; set; }

        public Providers Provider { get; set; }

        private string _redirectUrl;
        public string RedirectUrl
        {
            set
            {
                _redirectUrl = value;
            }
            get
            {
                return string.IsNullOrEmpty(_redirectUrl)
                                    ? HttpContext.Current.Request.Url.AbsoluteUri
                                    : _redirectUrl;
            }
        }

        public List<ClaimParameters> ClaimParameters;

        public List<FetchParameters> FetchParameters;

        public OAuthRequest()
        {
            Provider = Providers.Empty;
        }

        public void CreateRequest(FetchParameters parameters)
        {
            if (Provider == Providers.Empty)
                return;

            var requestString = new StringBuilder();
            requestString.Append(_providerEndPoint[Provider] + "?");
            requestString.Append(parameters.OpenidMode.RequestParameter());
            requestString.Append(parameters.OpenidNs.RequestParameter());
            requestString.Append(parameters.OpenidReturnTo.RequestParameter());
            requestString.Append(parameters.OpenidRealm.RequestParameter());
            requestString.Append(parameters.OpenidClaimedId.RequestParameter());
            requestString.Append(parameters.OpenidIdentity.RequestParameter());
            requestString.Append(parameters.OpenidNsAx.RequestParameter());
            requestString.Append(parameters.OpenidAxMode.RequestParameter());

            foreach (string info in parameters.OpenidUserInformation)
            {
                requestString.Append(info);
            }

            requestString.Append(parameters.OpenidAxRequired.RequestParameter());

            if (Provider == Providers.Google)
            {
                requestString.Append(string.Format("hl={0}", CultureInfo.CurrentCulture.TwoLetterISOLanguageName));
            }


            try
            {
                var request = WebRequest.Create(requestString.ToString());
                var respons = (HttpWebResponse)request.GetResponse();
                HttpContext.Current.Response.Redirect(respons.ResponseUri.AbsoluteUri, true);
            }
            catch (Exception ex)
            {
                if (!(ex is System.Threading.ThreadAbortException))
                {
                    Debug.LogError(ex);
                }
            }
        }

        public void CreateRequest(ClaimParameters parameters, bool withUserId)
        {
            if (Provider == Providers.Empty)
                return;

            var requestString = new StringBuilder();
            if (withUserId)
            {
                if (Provider == Providers.Mail)
                {
                    //var 
                    //if (userId.Contains("@"))
                    //{
                    //    userId = userId.Substring(0, userId.IndexOf("@"));
                    //}
                    var userIdAndDomainPair = UserId.Split(new[] { '@' });
                    parameters.OpenidClaimedId.Value = string.Format("http://{0}.id.{1}", userIdAndDomainPair[0], userIdAndDomainPair[1]);
                    parameters.OpenidIdentity.Value = string.Format("http://{0}.id.{1}", userIdAndDomainPair[0], userIdAndDomainPair[1]);
                }
            }

            requestString.Append(_providerEndPoint[Provider] + "?");
            requestString.Append(parameters.OpenidMode.RequestParameter());
            requestString.Append(parameters.OpenidNs.RequestParameter());
            requestString.Append(parameters.OpenidReturnTo.RequestParameter());
            requestString.Append(parameters.OpenidRealm.RequestParameter());
            requestString.Append(parameters.OpenidClaimedId.RequestParameter());
            requestString.Append(parameters.OpenidIdentity.RequestParameter());
            requestString.Append(parameters.OpenidNsSreg.RequestParameter());

            requestString.Append(parameters.OpenidSregRequired.RequestParameter());

            requestString.Append(parameters.OpenidSregOptional.RequestParameter());


            try
            {
                var request = WebRequest.Create(requestString.ToString());
                var respons = (HttpWebResponse)request.GetResponse();
                HttpContext.Current.Response.Redirect(respons.ResponseUri.AbsoluteUri, true);
            }
            catch (Exception ex)
            {
                if (!(ex is System.Threading.ThreadAbortException))
                {
                    Debug.LogError(ex);
                }
            }
        }
    }
}