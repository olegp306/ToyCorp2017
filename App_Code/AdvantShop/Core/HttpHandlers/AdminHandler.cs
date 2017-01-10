using System.Web;
using System.Web.SessionState;
using AdvantShop.Customers;
using AdvantShop.Security;


namespace AdvantShop.Core.HttpHandlers
{
    public abstract class AdminHandler : IHttpHandler, IRequiresSessionState
    {
        protected AdminHandler()
        {
            Localization.Culture.InitializeCulture();
        }

        public bool Authorize(HttpContext context)
        {
            if (CustomerContext.CurrentCustomer.IsAdmin
                || CustomerContext.CurrentCustomer.CustomerRole == Role.Moderator && RoleAccess.Check(CustomerContext.CurrentCustomer, context.Request.Url.ToString().ToLower())
                || CustomerContext.CurrentCustomer.IsVirtual
                || Trial.TrialService.IsTrialEnabled)
            {
                return true;
            }

            context.Response.Clear();
            context.Response.StatusCode = 403;
            context.Response.Status = "403 Forbidden";
            return false;
        }

        public void ProcessRequest(HttpContext context)
        {
            //throw new System.NotImplementedException();
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}