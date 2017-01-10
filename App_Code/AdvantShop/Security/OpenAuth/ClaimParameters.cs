//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Web;

namespace AdvantShop.Security.OpenAuth
{
    public class ClaimParameters : RequestParameters
    {
        public List<string> OpenidUserInformation;

        public parameter OpenidClaimedId;

        public parameter OpenidIdentity;

        public parameter OpenidNsSreg;

        public parameter OpenidSregOptional;

        public ClaimParameters()
        {

            OpenidReturnTo = new parameter
                                 {
                                     Name = "openid.return_to",
                                     Value = HttpContext.Current.Request.Url.AbsoluteUri
                                 };

            OpenidRealm = new parameter
                                {
                                    Name = "openid.realm",
                                    Value = HttpContext.Current.Request.Url.AbsoluteUri
                                };
            OpenidMode = new parameter
                             {
                                 Name = "openid.mode",
                                 Value = "checkid_setup"
                             };
            OpenidClaimedId = new parameter
                                  {
                                      Name = "openid.claimed_id",
                                      Value = "http://specs.openid.net/auth/2.0/identifier_select"
                                  };
            OpenidIdentity = new parameter
                                 {
                                     Name = "openid.identity",
                                     Value = "http://specs.openid.net/auth/2.0/identifier_select"
                                 };
            OpenidNsSreg = new parameter
                             {
                                 Name = "openid.ns.sreg",
                                 Value = "http://openid.net/extensions/sreg/1.1"
                             };
            OpenidSregOptional = new parameter
                                     {
                                         Name = "openid.sreg.optional",
                                         Value = string.Empty
                                     };
            OpenidNs = new parameter
                           {
                               Name = "openid.ns",
                               Value = "http://specs.openid.net/auth/2.0"
                           };
            OpenidSregRequired = new parameter
                                   {
                                       Name = "openid.sreg.required",
                                       Value = OpenidRequiredParams.email.ToString() + "," +
                                                OpenidRequiredParams.fullname.ToString()
                                   };
        }
    }
}