//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Web;

namespace AdvantShop.Security.OpenAuth
{
    public class RequestParameters
    {
        public enum InRequest
        {
            InRequets,
            NotInRequest
        }


        public struct parameter
        {
            public string Name;
            public string Value;

            public string RequestParameter()
            {
                return Name + "=" + HttpContext.Current.Server.UrlEncode(Value) + "&";
            }
        }

        public enum OpenidRequiredParams
        {
            country,
            email,
            firstname,
            language,
            lastname,
            fullname,
            gender
        }

        public struct AxSchemaParams
        {
            public struct Contact
            {
                public const string email = "openid.ax.type.email=http://axschema.org/contact/email&";
                public struct Country
                {
                    public const string home = "openid.ax.type.country=http://axschema.org/contact/country/home&";
                }
            }

            public struct NamePerson
            {
                public const string First = "openid.ax.type.firstname=http://axschema.org/namePerson/first&";
                public const string Last = "openid.ax.type.lastname=http://axschema.org/namePerson/last&";
            }

            public struct Preferences
            {
                public const string Language = "openid.ax.type.language=http://axschema.org/pref/language&";
            }
        }

        /// <summary>
        /// (required) Specifies the attribute being requested.
        /// </summary>
        public parameter OpenidAxRequired;

        public parameter OpenidSregRequired;

        public parameter OpenidReturnTo;

        public parameter OpenidRealm;

        public parameter OpenidNs;

        public parameter OpenidMode;

        public RequestParameters()
        {

        }
    }
}