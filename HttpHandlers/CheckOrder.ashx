<%@ WebHandler Language="C#" Class="CheckOrder" %>

using System.Web;
using Newtonsoft.Json;
using AdvantShop;
using AdvantShop.Orders;

public class CheckOrder : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        AdvantShop.Localization.Culture.InitializeCulture();
        
        string orderNum = context.Request["OrderNum"];

        context.Response.ContentType = "application/json";
        
        if (orderNum.IsNotEmpty())
        {
            StatusInfo statusInf = OrderService.GetStatusInfo(orderNum);
            context.Response.Write(JsonConvert.SerializeObject(statusInf ?? new StatusInfo { StatusComment = "", StatusName = Resources.Resource.Client_UserControls_StatusComment_NotFound }));
        }

        context.Response.End();
    }
    
    public bool IsReusable
    {
        get { return true;}
    }
}