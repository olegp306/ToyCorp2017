//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Web;

namespace AdvantShop.Security.OpenAuth
{
    public class FetchParameters : RequestParameters
    {
        public List<string> OpenidUserInformation;
        
        public parameter OpenidClaimedId;

        public parameter OpenidIdentity;

        public parameter OpenidNsAx;

        public parameter OpenidAxMode;

        public FetchParameters()
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
            OpenidAxMode = new parameter
                                  {
                                      Name = "openid.ax.mode",
                                      Value = "fetch_request"
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
            OpenidNsAx = new parameter
                             {
                                 Name = "openid.ns.ax",
                                 Value = "http://openid.net/srv/ax/1.0"
                             };
            OpenidNs = new parameter
                           {
                               Name = "openid.ns",
                               Value = "http://specs.openid.net/auth/2.0"
                           };
            OpenidAxRequired = new parameter
                                   {
                                       Name = "openid.ax.required",
                                       Value = OpenidRequiredParams.email.ToString() + "," +
                                                OpenidRequiredParams.firstname.ToString() + "," +
                                                OpenidRequiredParams.lastname.ToString()
                                   };

            OpenidUserInformation = new List<string>();

        }
    }
}