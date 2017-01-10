<%@ WebHandler Language="C#" Class="DeleteReview" %>

using System.Web;
using System.Web.SessionState;
using AdvantShop;
using AdvantShop.CMS;
using AdvantShop.Customers;
using Newtonsoft.Json;

public class DeleteReview : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {

        if (CustomerContext.CurrentCustomer.IsAdmin ||
            (CustomerContext.CurrentCustomer.IsModerator &&
             CustomerContext.CurrentCustomer.HasRoleAction(RoleActionKey.DisplayComments)))
        {

            int entityid = context.Request["entityid"].TryParseInt();
            if (entityid == 0)
            {
                JsonConvert.SerializeObject(false);
                return;
            }
            
            ReviewService.DeleteReview(entityid);
            JsonConvert.SerializeObject(true);
        }
        else 
        {
            JsonConvert.SerializeObject(false);
        }
    }

    
    public bool IsReusable
    {
        get { return true; }
    }
}